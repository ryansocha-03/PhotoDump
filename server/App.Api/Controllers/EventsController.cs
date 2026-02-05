using App.Api.Models;
using App.Api.Models.Request;
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
[Route("[controller]")]
public class EventsController(EventService eventService): ControllerBase
{
    [HttpGet("landing")]
    public async Task<ActionResult<EventLandingResponseModel>> GetEventLandingInfo(
        [FromHeader(Name = SessionConfiguration.EventHeaderName)] Guid eventPublicId)
    {
        var eventLandingData = await eventService.FetchLandingDetailsAsync(eventPublicId);
        if (eventLandingData is null) return NotFound($"No event with public id {eventPublicId}");
        
        return Ok(eventLandingData);
    }
    
    [Authorize(AuthenticationSchemes = "SessionScheme")]
    [HttpGet("guests")]
    public async Task<IActionResult> GuestListSearch( 
        [FromHeader(Name = SessionConfiguration.EventHeaderName)] Guid eventPublicIdHeader,
        [FromQuery] string guestName)
    {
        if (string.IsNullOrWhiteSpace(guestName) || guestName.Length < 3) return BadRequest("Guest name must be provided and at least 3 characters");
        
        return Ok(await eventService.FetchGuestSearchAsync(eventPublicIdHeader, guestName));
    }
}