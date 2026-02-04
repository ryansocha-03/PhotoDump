using Infrastructure.EntityFramework.Models;
using Infrastructure.EntityFramework.Repositories;
using Infrastructure.EntityFramework.Repositories.Interfaces;
using Internal.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Internal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventStatesController(IRepository<EventState> repository): ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<EventState>> GetEventState(int id)
    {
        var eventResult = await repository.GetAsync(id);
        if (eventResult is null)
            return NotFound("Event state not found");
        return Ok(eventResult);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventState>>> GetEventStates()
    {
        return Ok(await repository.GetAllAsync());
    }

    [HttpPost]
    public async Task<ActionResult<EventState>> CreateEventState(NewEventStateRequestModel newEventState)
    {
        if (string.IsNullOrWhiteSpace(newEventState.Name))
            return BadRequest("New event state name is required");

        var newEventStateEntity = new EventState()
        {
            StateName = newEventState.Name
        };

        try
        {
            await repository.AddAsync(newEventStateEntity);
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, "There was an error creating new event state. Check logs");
        }
        return Ok(newEventStateEntity);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<EventState>> DeleteEventState(int id)
    {
        var eventState = await repository.GetAsync(id);
        if (eventState is null)
            return NotFound("Event state not found");
        
        await repository.DeleteAsync(eventState.Id);
        return eventState;
    }
}