using App.Api.Models.Request;
using App.Api.Services;
using Core.Interfaces;
using Core.Models;
using Identity.Services.Sessions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MediaController(IContentStoreService contentStoreService, MediaService mediaService, EventService eventService) : ControllerBase
{
   [Authorize(AuthenticationSchemes = "SessionScheme")]
    [HttpGet("download")]
    public async Task<IActionResult> GetEventPublicPhotoDownload(
        [FromHeader(Name = SessionConfiguration.EventHeaderName)] Guid eventPublicId)
    {
        var downloadUrl =
            await contentStoreService.GeneratePresignedDownloadUrl(eventPublicId, FilePrivacyEnum.Public, "TestText.txt");
        
        return downloadUrl is null ? StatusCode(500, "Issue creating presigned download url.") : Ok(downloadUrl);
    }
    
    [HttpPost("upload")]
    public async Task<IActionResult> UploadMediaForEvent( 
        [FromHeader(Name = SessionConfiguration.EventHeaderName)] Guid eventPublicIdHeader,
        [FromBody] MediaUploadRequestModel mediaUploadData)
    {
        if (mediaUploadData.MediaUploadInfo.Count == 0)
            return BadRequest("No uploads? Alright idiot.");

        var eventData = await eventService.FetchLandingDetailsAsync(eventPublicIdHeader);
        if  (eventData is null)
            return NotFound("No event found.");
        
        // write files to database
        var publicFileNames = await mediaService.UploadMedia(mediaUploadData.MediaUploadInfo,
            eventData.Id,
            mediaUploadData.IsPrivate);
        
        return Ok(publicFileNames); 
        // generate and return presigned URLs 
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
    public async Task<IActionResult> ListMediaForEvent([FromRoute] int eventId)
    {
        return Ok(await mediaService.GetMediaForEvent(eventId));
    }
}