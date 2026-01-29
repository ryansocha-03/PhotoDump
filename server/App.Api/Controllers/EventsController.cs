using App.Api.Models;
using App.Api.Models.Response;
using App.Api.Services;
using Core.Interfaces;
using Core.Models;
using Identity.Models;
using Identity.Services;
using Identity.Services.Sessions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController(EventService eventService, PasswordService passwordService, SessionService sessionService, IContentStoreService contentStoreService): ControllerBase
{
    [HttpGet("{eventPublicId}")]
    public async Task<ActionResult<EventLandingResponseModel>> GetEventLandingInfo([FromRoute] Guid eventPublicId)
    {
        var eventLandingData = await eventService.FetchLandingDetailsAsync(eventPublicId);
        if (eventLandingData is null) return NotFound($"No event with public id {eventPublicId}");
        
        return Ok(eventLandingData);
    }

    [HttpPost("{eventPublicId}/auth")]
    public async Task<IActionResult> EventLogIn([FromRoute] Guid eventPublicId,
        [FromBody] EventLoginRequestModel eventLoginRequest)
    {
        if (string.IsNullOrWhiteSpace(eventLoginRequest.EventKey))
            return BadRequest("Event key is required");
        
        var eventHash = await eventService.FetchEventHashAsync(eventPublicId);
        if (eventHash is null) return NotFound($"No event with public id {eventPublicId}");

        if (passwordService.PasswordHashMatches(eventLoginRequest.EventKey, eventHash))
        {
            var sessionData = await sessionService.CreateSessionAsync(eventPublicId);
            return Ok(new SessionResponseModel(){ SessionId = sessionData.Item1.ToString(), ExpiresAt = sessionData.Item2 });
        }

        return Unauthorized("Invalid event key.");
    }
    
    [HttpGet("{eventPublicId}/validate")]
    public async Task<IActionResult> ValidateEventSession([FromRoute] Guid eventPublicId,
        [FromHeader(Name = SessionConfiguration.SessionHeaderName)] Guid sessionIdHeader,
        [FromHeader(Name = SessionConfiguration.EventHeaderName)] Guid eventPublicIdHeader)
    {
        if (!eventPublicIdHeader.Equals(eventPublicId))
            return BadRequest("Event ID is invalid.");

        var sessionType = await sessionService.GetSessionTypeAsync(sessionIdHeader, eventPublicIdHeader);
        if (sessionType is null)
            return Unauthorized("No valid session for provided session id.");

        return Ok(new SessionTypeModel
        {
            SessionType = sessionType.Value
        });
    }

    [Authorize(AuthenticationSchemes = "SessionScheme")]
    [HttpPost("{eventPublicId}/auth/guest")]
    public async Task<IActionResult> LoginGuestForEvent([FromRoute] Guid eventPublicId,
        [FromHeader(Name = SessionConfiguration.SessionHeaderName)]
        Guid sessionIdHeader,
        [FromHeader(Name = SessionConfiguration.EventHeaderName)]
        Guid eventPublicIdHeader,
        [FromBody] GuestLoginRequestModel guestData)
    {
        if (!eventPublicIdHeader.Equals(eventPublicId)) 
            return BadRequest("Event ID is invalid.");

        var upgradeStatus =
            await sessionService.UpgradeSessionToGuestAsync(sessionIdHeader, eventPublicIdHeader, guestData.GuestId);

        if (!upgradeStatus)
            return BadRequest("You gave some DUMB shit!");
        
        return Ok();
    }

    [Authorize(AuthenticationSchemes = "SessionScheme")]
    [HttpGet("{eventPublicId}/guests")]
    public async Task<IActionResult> GuestListSearch([FromRoute] Guid eventPublicId, 
        [FromHeader(Name = SessionConfiguration.EventHeaderName)] Guid eventPublicIdHeader,
        [FromQuery] string guestName)
    {
        if (string.IsNullOrWhiteSpace(guestName) || guestName.Length < 3) return BadRequest("Guest name must be provided and at least 3 characters");
        
        return Ok(await eventService.FetchGuestSearchAsync(eventPublicIdHeader, guestName));
    }

    [Authorize(AuthenticationSchemes = "SessionScheme")]
    [HttpGet("{eventPublicId}/media/download")]
    public async Task<IActionResult> GetEventPublicPhotoDownload([FromRoute] Guid eventPublicId,
        [FromHeader(Name = SessionConfiguration.EventHeaderName)] Guid eventPublicIdHeader)
    {
        var downloadUrl =
            await contentStoreService.GeneratePresignedDownloadUrl(eventPublicIdHeader, FilePrivacyEnum.Public, "TestText.txt");
        
        return downloadUrl is null ? StatusCode(500, "Issue creating presigned download url.") : Ok(downloadUrl);
    }
    
    [Authorize(AuthenticationSchemes = "SessionScheme")]
    [HttpGet("{eventPublicId}/media/upload/public")]
    public async Task<IActionResult> GetPublicMediaUpload([FromRoute] Guid eventPublicId, 
        [FromHeader(Name = SessionConfiguration.EventHeaderName)] Guid eventPublicIdHeader)
    {
        var uploadUrl = await contentStoreService.GeneratePresignedUploadUrl(eventPublicIdHeader, FilePrivacyEnum.Public, "TestText.txt");

        return uploadUrl is null ? StatusCode(500, "Issue creating presigned upload url.") : Ok(uploadUrl);
    }

    [Authorize(AuthenticationSchemes = "SessionScheme")]
    [HttpGet("{eventPublicId}/media/upload/private")]
    public async Task<IActionResult> GetPrivateMediaUpload([FromRoute] Guid eventPublicId,
        [FromHeader(Name = SessionConfiguration.EventHeaderName)]
        Guid eventPublicIdHeader)
    {
        var uploadUrl = await contentStoreService.GeneratePresignedUploadUrl(eventPublicIdHeader, FilePrivacyEnum.Private, "TestText.txt");

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