using Infrastructure.EntityFramework.Models;
using Infrastructure.EntityFramework.Repositories;
using Infrastructure.EntityFramework.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Internal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController(IEventRepository repository): ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Event>>> GetAllEvents()
    {
        return Ok(await repository.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Event>> GetEventById(int id)
    {
        var eventResult = await repository.GetAsync(id);
        if (eventResult is null)
            return NotFound($"Event with id: {id} not found");
        
        return eventResult;
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Event>> DeleteEvent(int id)
    {
        var eventResult = await repository.GetAsync(id);
        if (eventResult is null)
            return NotFound($"Event with id: {id} not found");
        
        await repository.DeleteAsync(id);
        return eventResult;
    }
}