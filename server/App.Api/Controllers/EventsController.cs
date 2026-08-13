using App.Api.Constants;
using App.Api.Models.Response;
using App.Api.Services.Definition;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers;

/// <summary>
/// Provides endpoints related to events.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/[controller]")]
public class EventsController(IEventService eventService, ILogger<EventsController> logger) : ControllerBase
{
    /// <summary>
    /// Gets overview information for the provided event.
    /// </summary>
    [HttpGet]
    [ActionName("GetEventOverviewInfo")]
    [ProducesResponseType(typeof(EventOverviewResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetEventOverviewInfo(
        [FromHeader(Name = SessionAuthHeaders.EventHeader)] Guid eventPublicId)
    {
        if (eventPublicId == Guid.Empty)
            return BadRequest("Event Id cannot be empty");

        try
        {
            var eventLandingData = await eventService.GetEventOverviewAsync(eventPublicId);
            if (eventLandingData is null) return NotFound($"Event {eventPublicId} not found");

            return Ok(eventLandingData);
        }
        catch (Exception ex)
        {
            logger.LogError($"Exception when getting event overview details for event {eventPublicId}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occured.");
        }

    }
}