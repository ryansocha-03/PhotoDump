using App.Api.Constants;
using App.Api.Models.DTOs;
using App.Api.Models.Request;
using App.Api.Models.Response;
using App.Api.Services.Definition;
using Asp.Versioning;
using ContentStore.Abstractions.Interfaces;
using ContentStore.Abstractions.Models;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers;

/// <summary>
/// Provides endpoints for media operations.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize(AuthenticationSchemes = AuthSchemes.SessionAuth)]
[Route("v{version:apiVersion}/[controller]")]
public class MediaController(
    IContentStoreService contentStoreService, 
    IMediaService mediaService, 
    IEventService eventService, 
    ILogger<MediaController> logger,
    IBrokerPublisher publisher) : ControllerBase
{
    
    /// <summary>
    /// Gets a paginated list of public media presigned download URLs based off the provided paging parameters.
    /// </summary>
    [HttpGet("download")]
    [ActionName("GetEventPublicMediaDownload")]
    [ProducesResponseType(typeof(PaginationDto<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetEventPublicMediaDownload(
        [FromHeader(Name = SessionAuthHeaders.EventHeader)] Guid eventPublicIdHeader,
        [FromQuery] string? cursor,
        [FromQuery] int? limit)
    {
        var eventId = await eventService.GetEventInternalIdAsync(eventPublicIdHeader);
        
        if (eventId == 0)
            return NotFound("Event not found.");

        PaginationDto<string> mediaPageData;
        try
        {
            mediaPageData = await mediaService.GetEventPublicMediaPagedAsync(eventId, eventPublicIdHeader, cursor, limit);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting event media");
            return StatusCode(500, "Unexpected error when getting media.");
        }

        if (!mediaPageData.Items.Any())
            return Ok(new PaginationDto<string>());

        IEnumerable<string> urls;
        try
        {
            urls = await contentStoreService.CreateDownloadsAsync(
                new ContentKeyGroup(
                    eventPublicIdHeader, 
                    FilePrivacyEnum.Public, 
                    ContentVariantEnum.Gallery, 
                    mediaPageData.Items));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating download URLs.");
            return StatusCode(500, "Unexpected error when generating downloads.");
        }
        
        return Ok(new PaginationDto<string>
        {
            Items = urls,
            HasNext = mediaPageData.HasNext,
            NextCursor = mediaPageData.NextCursor
        });
    }
    
    [HttpPost("upload")]
    public async Task<IActionResult> UploadMediaForEvent( 
        [FromHeader(Name = SessionAuthHeaders.EventHeader)] Guid eventPublicIdHeader,
        [FromBody] MediaUploadRequestModel mediaUploadData)
    {
        if (mediaUploadData.MediaUploadInfo.Count == 0)
            return BadRequest("No uploads? Alright idiot.");

        var eventId = await eventService.GetEventIdByPublicId(eventPublicIdHeader);
        if  (eventId is not { } resolvedEventId)
            return NotFound("No event found.");
        
        // write files to database
        List<string> publicFileNames;
        try
        {
            publicFileNames = await mediaService.UploadMedia(mediaUploadData.MediaUploadInfo,
                resolvedEventId,
                mediaUploadData.IsPrivate);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return StatusCode(500, "Unexpected error occured while uploading media."); 
        }

        if (publicFileNames.Count == 0)
            return BadRequest("Unsupported file type(s).");
        
        // generate and return presigned URLs 
        List<string> urls;
        try
        {
            urls = (await contentStoreService.GenerateBulkPresignedUploadUrls(
                publicFileNames, 
                eventPublicIdHeader,
                mediaUploadData.IsPrivate ? FilePrivacyEnum.Private : FilePrivacyEnum.Public)).ToList();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return StatusCode(500, "Unexpected error occured while generating uploads.");
        }

        if (publicFileNames.Count != urls.Count)
            return StatusCode(500);
            
        var uploadResponses = publicFileNames.Select(
            (t, i) => new MediaUploadResponseModel { FileUploadUrl = urls[i], PublicFileId = t });

        return Ok(uploadResponses);
    }

    [Authorize(AuthenticationSchemes = "SessionScheme")]
    [HttpPost("upload/{publicFileId}/complete")]
    public async Task<IActionResult> AcknowledgeCompletedUpload([FromRoute] string publicFileId,
        [FromHeader(Name = SessionConfiguration.EventHeaderName)] Guid eventPublicIdHeader)
    {
        var numUpdated = await mediaService.AcknowledgeUploadStateTransition(publicFileId, eventPublicIdHeader);

        MediaStateTransitionDto uploadedMedia;
        switch  (numUpdated.Count)
        {
            case 0:
                return NoContent();
            case 1:
                uploadedMedia = numUpdated[0];
                break;
            default:
                return StatusCode(500);
        }

        await publisher.PublishAsync("photo-thumbnail", new ProcessMediaThumbnailMessageModel
        {
            ObjectName = contentStoreService.BuildObjectName(
                eventPublicIdHeader, 
                uploadedMedia.IsPrivate ? FilePrivacyEnum.Private : FilePrivacyEnum.Public,
                publicFileId 
            ),
            MediaId = uploadedMedia.MediaInternalId
        });

        return Ok();
    }
}