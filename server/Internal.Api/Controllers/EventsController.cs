using Asp.Versioning;
using Domain.Entities;
using Domain.Enums;
using Internal.Api.Models.Request;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Persistence.Abstractions.Interfaces.Repositories;

namespace Internal.Api.Controllers;

/// <summary>
/// Provides endpoints for internal actions on events.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class EventsController(IEventRepository repository, IPasswordHasher<object> passwordHasher): ControllerBase
{
    /// <summary>
    /// Retrieves event entities for all events.
    /// </summary>
    [HttpGet]
    [ActionName("GetAllEvents")]
    [ProducesResponseType(typeof(IReadOnlyCollection<Event>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllEvents()
    {
        try
        {
            return Ok(await repository.GetAllAsync());
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Unexpected error occured while getting all events:  {ex.Message}");
        }
    }

    /// <summary>
    /// Retrieves the event entity for the provided internal identifier.
    /// </summary>
    [HttpGet("{id:long}")]
    [ActionName("GetEventById")]
    [ProducesResponseType(typeof(Event), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetEventById([FromRoute] long id)
    {
        Event? eventResult;
        try
        {
            eventResult = await repository.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Unexpected error occured while getting event: {ex.Message}");
        }
        
        if (eventResult == null)
            return NotFound($"Event with id: {id} not found");
        
        return Ok(eventResult);
    }

    /// <summary>
    /// Retrieves the event entity for the provided public identifier.
    /// </summary>
    [HttpGet("public/{id:guid}")]
    [ActionName("GetEventByPublicId")]
    [ProducesResponseType(typeof(Event), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetEventByPublicId([FromRoute] Guid id)
    {
        Event? eventResult;
        try
        {
            eventResult = await repository.GetByPublicIdAsync(id);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Unexpected error occured while getting event by public ID: {ex.Message}");
        }
        
        if (eventResult == null)
            return NotFound($"Event with public id: {id} not found");
        
        return Ok(eventResult);
    }

    /// <summary>
    /// Creates a new event entity.
    /// </summary>
    [HttpPost]
    [ActionName("CreateEvent")]
    [ProducesResponseType(typeof(Event), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateEvent([FromBody] NewEventRequestModel newEventData)
    {
        var hashedPassword = passwordHasher.HashPassword(new {}, newEventData.EventPassword);

        var newEvent = new Event
        {
            EventName = newEventData.EventName,
            EventNameShort = newEventData.EventNameShort,
            ColorPrimary = newEventData.ColorPrimary,
            ColorSecondary = newEventData.ColorSecondary,
            StartDate = newEventData.EventStartDate.ToUniversalTime(),
            EndDate = newEventData.EventEndDate.ToUniversalTime(),
            EventPasswordHash = hashedPassword,
            EventState = EventStateEnum.Published,
            EventTypeId = newEventData.EventTypeId
        };

        try
        {
            return Ok(await repository.CreateAsync(newEvent));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Unexpected error occurred when creating new event: {ex.Message}");
        }
    }

    /// <summary>
    /// Updates an event entity.
    /// </summary>
    [HttpPut("{id:long}")]
    [ActionName("UpdateEvent")]
    [ProducesResponseType(typeof(Event), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateEvent([FromRoute] long id, [FromBody] UpdateEventRequestModel updateEventData)
    {
        var eventToUpdate = await repository.GetByIdAsync(id);
        if (eventToUpdate == null)
            return NotFound($"Event with id: {id} not found");
        
        var hashedPassword = string.IsNullOrWhiteSpace(updateEventData.EventPassword)
            ? eventToUpdate.EventPasswordHash
            : passwordHasher.HashPassword(new {}, updateEventData.EventPassword);
        
        eventToUpdate.EventName = updateEventData.EventName ??  eventToUpdate.EventName;
        eventToUpdate.EventNameShort = updateEventData.EventNameShort ??  eventToUpdate.EventNameShort;
        eventToUpdate.ColorPrimary = updateEventData.ColorPrimary ??  eventToUpdate.ColorPrimary;
        eventToUpdate.ColorSecondary = updateEventData.ColorSecondary ?? eventToUpdate.ColorSecondary;
        eventToUpdate.StartDate = updateEventData.EventStartDate?.ToUniversalTime() ??  eventToUpdate.StartDate;
        eventToUpdate.EndDate = updateEventData.EventEndDate?.ToUniversalTime() ??  eventToUpdate.EndDate;
        eventToUpdate.EventPasswordHash = hashedPassword;
        eventToUpdate.EventState = updateEventData.EventState == 0 ? eventToUpdate.EventState : updateEventData.EventState;
        eventToUpdate.EventTypeId = updateEventData.EventTypeId == 0 ? eventToUpdate.EventTypeId : updateEventData.EventTypeId;
        
        return Ok(await repository.UpdateAsync(eventToUpdate));
    }
    
    /// <summary>
    /// Deletes the event for the provided internal identifier.
    /// </summary>
    [HttpDelete("{id:long}")]
    [ActionName("DeleteEvent")]
    [ProducesResponseType(typeof(Event), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteEvent([FromRoute] long id)
    {
        try
        {
            var eventResult = await repository.GetByIdAsync(id);
            if (eventResult == null)
                return NotFound($"Event with id: {id} not found");

            await repository.DeleteAsync(id);
            return Ok(eventResult);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Unexpected error occured while deleting event: {ex.Message}");
        }
    }
}