using App.Api.Models.Request;
using Core.Interfaces;
using Core.Models;
using Identity.Services.Sessions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MediaController(IContentStoreService contentStoreService) : ControllerBase
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
    
    [Authorize(AuthenticationSchemes = "SessionScheme")]
    [HttpGet("upload")]
    public async Task<IActionResult> UploadMediaForEvent( 
        [FromHeader(Name = SessionConfiguration.EventHeaderName)] Guid eventPublicIdHeader,
        [FromBody] MediaUploadRequestModel mediaUploadData)
    {
        if (mediaUploadData.MediaUploadInfo.Count == 0)
            return BadRequest("No uploads? Alright idiot.");
        // write files to database
        // generate and return presigned URLs 
        var uploadUrl = await contentStoreService.GeneratePresignedUploadUrl(eventPublicIdHeader, FilePrivacyEnum.Public, "TestText.txt");

        return uploadUrl is null ? StatusCode(500, "Issue creating presigned upload url.") : Ok(uploadUrl);
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
}