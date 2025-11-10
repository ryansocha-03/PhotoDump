using Infrastructure.EntityFramework.Models;
using Infrastructure.EntityFramework.Repositories;
using Internal.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Internal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventTypesController(IRepository<EventType> repository): ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<EventType>> GetEventType(int id)
    {
        var eventResult = await repository.GetAsync(id);
        if (eventResult is null)
            return NotFound("Event type not found");
        return Ok(eventResult);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventType>>> GetEventTypes()
    {
        return Ok(await repository.GetAllAsync());
    }

    [HttpPost]
    public async Task<ActionResult<EventType>> CreateEventState(NewEventTypeRequestModel newEventType)
    {
        if (string.IsNullOrWhiteSpace(newEventType.Name))
            return BadRequest("New event type name is required");

        var newEventTypeEntity = new EventType()
        {
            TypeName = newEventType.Name
        };

        try
        {
            await repository.AddAsync(newEventTypeEntity);
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, "There was an error creating new event type. Check logs");
        }
        return Ok(newEventTypeEntity);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<EventType>> DeleteEventType(int id)
    {
        var eventType = await repository.GetAsync(id);
        if (eventType is null)
            return NotFound("Event type not found");
        
        await repository.DeleteAsync(eventType.Id);
        return eventType;
    }   
}