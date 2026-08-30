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
[Route("api/v{version:apiVersion}/[controller]")]
public class MediaController(
    IContentStoreService contentStoreService, 
    IMediaService mediaService, 
    IEventService eventService, 
    ILogger<MediaController> logger) : ControllerBase
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
    
    /// <summary>
    /// Generates a sequence of presigned upload URLs for the provided request objects.
    /// </summary>
    [HttpPost("upload")]
    [ActionName("UploadMediaForEvent")]
    [ProducesResponseType(typeof(IEnumerable<MediaUploadResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadMediaForEvent( 
        [FromHeader(Name = SessionAuthHeaders.EventHeader)] Guid eventPublicIdHeader,
        [FromBody] MediaUploadRequestModel mediaUploadData)
    {
        if (mediaUploadData.MediaUploadInfo.Count == 0)
            return BadRequest("No uploads? Alright idiot.");

        var eventId = await eventService.GetEventInternalIdAsync(eventPublicIdHeader);
        if  (eventId == 0)
            return NotFound("No event found.");
        
        // write files to database
        List<string> publicFileNames;
        try
        {
            publicFileNames = (await mediaService.CreateNewMediaEntriesAsync(
                eventId,
                mediaUploadData.Privacy,
                mediaUploadData.MediaUploadInfo)).ToList();
        }
        catch (ArgumentException ex)
        {
            logger.LogError($"One of the proposed new media was invalid: {ex.Message}");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError($"Unexpected error when uploading media: {ex}");
            return StatusCode(500, "Unexpected error occured while uploading media."); 
        }

        // generate and return presigned URLs 
        IReadOnlyList<string> urls;
        try
        {
            urls = await contentStoreService.CreateUploadsAsync(
                new ContentKeyGroup(eventPublicIdHeader, mediaUploadData.Privacy, ContentVariantEnum.Original, publicFileNames));
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return StatusCode(500, "Unexpected error occured while generating uploads.");
        }

        var uploadResponses = publicFileNames.Select(
            (t, i) => new MediaUploadResponseModel { FileUploadUrl = urls[i], PublicFileId = t });

        return Ok(uploadResponses);
    }
}