using App.Api.Models.Request;
using App.Api.Models.Response;
using App.Api.Services;
using App.Api.Services.DTOs;
using Core.DTOs;
using Core.Interfaces;
using Core.Models;
using Identity.Services.Sessions;
using Infrastructure.EntityFramework.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MediaController(IContentStoreService contentStoreService, MediaService mediaService, EventService eventService, IBrokerPublisher publisher) : ControllerBase
{
    [Authorize(AuthenticationSchemes = "SessionScheme")]
    [HttpGet("download")]
    public async Task<IActionResult> GetEventPublicMediaDownload(
        [FromHeader(Name = SessionConfiguration.EventHeaderName)] Guid eventPublicIdHeader,
        [FromQuery] string? cursor,
        [FromQuery] int? limit)
    {
        var eventId = await eventService.GetEventIdByPublicId(eventPublicIdHeader);
        if (eventId is not { } resolvedEventId)
            return NotFound("No event found.");

        PaginationDto<MediaNameDto> mediaPageData;
        try
        {
            mediaPageData = await mediaService.GetPublicMediaPageAsync(resolvedEventId, eventPublicIdHeader, cursor, limit);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return StatusCode(500, "Unexpected error when getting media.");
        }

        if (mediaPageData.Items.Count == 0)
            return Ok(mediaPageData);

        IEnumerable<string> urls;
        try
        {
            urls = await contentStoreService.GenerateBulkPresignedDownloadUrls(
                mediaPageData.Items,
                eventPublicIdHeader,
                FilePrivacyEnum.Public,
                "_gallery");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return StatusCode(500, "Unexpected error when generating downloads.");
        }
        
        return Ok(new PaginationDto<string>
        {
            Items = urls.ToList(),
            HasNext = mediaPageData.HasNext,
            NextCursor = mediaPageData.NextCursor
        });
    }
    
    [Authorize(AuthenticationSchemes = "SessionScheme")]
    [HttpPost("upload")]
    public async Task<IActionResult> UploadMediaForEvent( 
        [FromHeader(Name = SessionConfiguration.EventHeaderName)] Guid eventPublicIdHeader,
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

    [HttpGet("buckets")]
    public async Task<IActionResult> ListBuckets()
    {
        var buckets = await contentStoreService.ListBuckets();
        return Ok(buckets);
    }

    [Authorize(AuthenticationSchemes = "SessionScheme")]
    [HttpGet("buckets/list")]
    public async Task<IActionResult> ListObjects(
        [FromHeader(Name = SessionConfiguration.EventHeaderName)] Guid eventPublicIdHeader)
    {
        var items = await contentStoreService.ListObjectsInBucket(eventPublicIdHeader);
        
        return Ok(items);
    } 
    
    [HttpGet("list/{eventId}")]
    public IActionResult ListMediaForEvent([FromRoute] int eventId)
    {
        return Ok(mediaService.GetAllMediaForEvent(eventId));
    }

    [HttpGet("delete/{mediaId}")]
    public async Task<IActionResult> DeleteMediaFromEvent([FromRoute] int mediaId,
        [FromHeader(Name = SessionConfiguration.EventHeaderName)] Guid eventPublicIdHeader)
    {  
        var media = await mediaService.GetSpecificMedia(mediaId);
        if (media is null)
            return NotFound("No media found.");
        
        await mediaService.DeleteMedia(mediaId);
        await contentStoreService.DeleteMediaFromEvent(
            eventPublicIdHeader, 
            media.IsPrivate ?  FilePrivacyEnum.Private : FilePrivacyEnum.Public,
            media.PublicFileName);

        return Ok();
    }
}