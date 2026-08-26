using Asp.Versioning;
using Domain.Entities;
using Internal.Api.Models.Request;
using Microsoft.AspNetCore.Mvc;
using Persistence.Abstractions.Interfaces.Repositories;

namespace Internal.Api.Controllers;

/// <summary>
/// Provides endpoints for internal operations on event types.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class EventTypeController(IEventTypeRepository eventTypeRepository) : ControllerBase
{
    /// <summary>
    /// Retrieves all event type entities.
    /// </summary>
    [HttpGet]
    [ActionName("GetAllEventTypes")]
    [ProducesResponseType(typeof(IReadOnlyCollection<EventType>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string),  StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            return Ok(await eventTypeRepository.GetAllAsync());
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Unexpected error occured: {ex.Message}");
        }
    }

    /// <summary>
    /// Retrieves the event type entity for the provided internal identifier.
    /// </summary>
    [HttpGet("{id:long}")]
    [ActionName("GetEventById")]
    [ProducesResponseType(typeof(EventType), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetEventById([FromRoute] long id)
    {
        EventType? eventType;
        try
        {
            eventType = await eventTypeRepository.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Unexpected error occured: {ex.Message}");
        }
        
        if (eventType == null)
            return NotFound($"No event type found with id: {id}");
        
        return Ok(eventType);
    }

    /// <summary>
    /// Creates a new event type with the provided name.
    /// </summary>
    [HttpPost]
    [ActionName("CreateEventType")]
    [ProducesResponseType(typeof(EventType), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateEventType([FromBody] EventTypeRequestModel eventTypeData)
    {
        if (string.IsNullOrWhiteSpace(eventTypeData.EventTypeName))
            return BadRequest("Event type name is required");

        var newEventType = new EventType
        {
            TypeName = eventTypeData.EventTypeName
        };

        try
        {
            return Ok(await eventTypeRepository.CreateAsync(newEventType));
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Unexpected error when creating new event type: {ex.Message}");
        }
    }

    /// <summary>
    /// Updates the name of the event type with the provided internal identifier.
    /// </summary>
    [HttpPut("{id:long}")]
    [ActionName("UpdateEventTypeName")]
    [ProducesResponseType(typeof(EventType), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateEventTypeName([FromRoute] long id,
        [FromBody] EventTypeRequestModel eventTypeData)
    {
        if (string.IsNullOrWhiteSpace(eventTypeData.EventTypeName))
            return BadRequest("Event type name is required");

        try
        {
            var eventToUpdate = await eventTypeRepository.GetByIdAsync(id);
            if (eventToUpdate == null)
                return NotFound($"No event type found with id: {id}");

            eventToUpdate.TypeName = eventTypeData.EventTypeName;

            return Ok(await eventTypeRepository.UpdateAsync(eventToUpdate));
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Unexpected error when updating event type: {ex.Message}");
        }
    }

    /// <summary>
    /// Deletes the event type with the provided internal identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id:long}")]
    [ActionName("DeleteEventType")]
    [ProducesResponseType(typeof(EventType), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteEventType([FromRoute] long id)
    {
        try
        {
            var eventToDelete = await eventTypeRepository.GetByIdAsync(id);
            if (eventToDelete == null)
                return NotFound($"No event type found with id: {id}");

            await eventTypeRepository.DeleteAsync(id);
            return Ok(eventToDelete);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Unexpected error when deleting event type: {ex.Message}");
        }
    }
}