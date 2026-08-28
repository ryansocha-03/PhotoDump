using Asp.Versioning;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Persistence.Abstractions.Interfaces.Repositories;

namespace Internal.Api.Controllers;

/// <summary>
/// Provides endpoints for internal actions on events.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class EventSessionsController(IEventSessionRepository eventSessionRepository) : ControllerBase
{
    /// <summary>
    /// Gets all event sessions for the provided event.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ActionName("GetEventSessionsForEvent")]
    [ProducesResponseType(typeof(IEnumerable<EventSession>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetEventSessionsForEventAsync([FromRoute] Guid id)
    {
        try
        {
            return Ok(await eventSessionRepository.GetAllAsync(id));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Unexpected error when getting list of event sessions: {ex.Message}");
        }
    }

    /// <summary>
    /// Deletes all invalid event sessions.
    /// </summary>
    [HttpDelete("invalid")]
    [ActionName("DeleteAllInvalidEventSessions")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteAllInvalidEventSessionsAsync()
    {
        try
        {
            return Ok(await eventSessionRepository.DeleteInvalidAsync());
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Unexpected error when deleting invalid event sessions: {ex.Message}");
        }
    }
}