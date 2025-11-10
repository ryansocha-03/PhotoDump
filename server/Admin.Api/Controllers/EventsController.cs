using Admin.Api.Models;
using Identity.Services;
using Infrastructure.EntityFramework.Models;
using Infrastructure.EntityFramework.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Admin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController(PasswordService passwordService, IRepository<Event> eventRepository) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateNewEvent([FromBody] NewEventRequestModel newEvent)
    {
        var passwordVerificationResult = passwordService.VerifyPasswordRequirements(newEvent.EventPassword);
        if (passwordVerificationResult is not null)
            return BadRequest(passwordVerificationResult);
        
        var hashedPassword = passwordService.HashPassword(newEvent.EventPassword);
        Console.WriteLine(hashedPassword);

        var eventEntity = new Event()
        {
            EventName = newEvent.EventName,
            EventDate = newEvent.EventDate,
            EventStateId = newEvent.EventStateId,
            EventTypeId = newEvent.EventTypeId,
            EventPasswordHash = hashedPassword,
            DurationDays = newEvent.DurationDays,
        };
        
        var addedEvent = await eventRepository.AddAsync(eventEntity);
        
        return Ok(addedEvent);
    }
}