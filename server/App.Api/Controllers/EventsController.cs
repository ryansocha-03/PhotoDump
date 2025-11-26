using App.Api.Models;
using App.Api.Models.Response;
using App.Api.Services;
using Core.Interfaces;
using Identity.Services;
using Infrastructure.EntityFramework.Models;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController(EventService eventService, PasswordService passwordService, TokenService tokenService, IContentStoreService contentStoreService): ControllerBase
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
            return Ok(tokenService.CreateAnonymousGuestToken(eventPublicId));
        return Unauthorized("Invalid event key.");
    }

    [HttpGet("{eventPublicId}/guests")]
    public async Task<IActionResult> GuestListSearch([FromRoute] Guid eventPublicId, 
        [FromQuery] string guestName)
    {
        if (string.IsNullOrWhiteSpace(guestName) || guestName.Length < 3) return BadRequest("Guest name must be provided and at least 3 characters");
        
        return Ok(await eventService.FetchGuestSearchAsync(eventPublicId, guestName));
    }

    [HttpGet("buckets")]
    public async Task<IActionResult> ListBuckets()
    {
        await contentStoreService.ListBuckets();
        return Ok();
    }
}