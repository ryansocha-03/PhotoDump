using App.Api.Models;
using App.Api.Models.Response;
using App.Api.Services;
using Identity.Models;
using Identity.Services;
using Identity.Services.Sessions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(
    EventService eventService, 
    PasswordService passwordService,
    SessionService sessionService) 
    : ControllerBase
{
    [HttpPost("event")]
    public async Task<IActionResult> EventLogIn(
        [FromHeader(Name = SessionConfiguration.EventHeaderName)] Guid eventPublicId,
        [FromBody] EventLoginRequestModel eventLoginRequest)
    {
        if (string.IsNullOrWhiteSpace(eventLoginRequest.EventKey) || eventPublicId == Guid.Empty)
            return BadRequest("Missing required fields.");
        
        var eventHash = await eventService.FetchEventHashAsync(eventPublicId);
        if (eventHash is null) return NotFound($"No event with public id {eventPublicId}");

        if (passwordService.PasswordHashMatches(eventLoginRequest.EventKey, eventHash))
        {
            var sessionData = await sessionService.CreateSessionAsync(eventPublicId);
            return Ok(new SessionResponseModel{ SessionId = sessionData.Item1.ToString(), ExpiresAt = sessionData.Item2 });
        }

        return Unauthorized("Invalid event key.");
    }    
    
    [HttpGet("validate")]
    public async Task<IActionResult> ValidateEventSession(
        [FromHeader(Name = SessionConfiguration.SessionHeaderName)] Guid sessionIdHeader,
        [FromHeader(Name = SessionConfiguration.EventHeaderName)] Guid eventPublicIdHeader)
    {
        var sessionType = await sessionService.GetSessionTypeAsync(sessionIdHeader, eventPublicIdHeader);
        if (sessionType is null)
            return Unauthorized("No valid session for provided session id.");

        return Ok(new SessionTypeModel 
        {
            SessionType = sessionType.Value
        });
    }
    
    [Authorize(AuthenticationSchemes = "SessionScheme")]
    [HttpPost("guest")]
    public async Task<IActionResult> LoginGuestForEvent(
        [FromHeader(Name = SessionConfiguration.SessionHeaderName)] Guid sessionIdHeader,
        [FromHeader(Name = SessionConfiguration.EventHeaderName)] Guid eventPublicIdHeader,
        [FromBody] GuestLoginRequestModel guestData)
    {
        var upgradeStatus =
            await sessionService.UpgradeSessionToGuestAsync(sessionIdHeader, eventPublicIdHeader, guestData.GuestId);

        if (!upgradeStatus)
            return BadRequest("You gave some DUMB shit!");
        
        return Ok();
    }
}