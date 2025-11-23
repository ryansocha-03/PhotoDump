using App.Api.Models;
using App.Api.Models.Response;
using App.Api.Services;
using Identity.Services;
using Infrastructure.EntityFramework.Models;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController(EventService eventService, PasswordService passwordService, TokenService tokenService): ControllerBase
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
}