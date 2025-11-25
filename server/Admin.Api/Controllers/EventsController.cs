using Admin.Api.Models;
using Identity.Services;
using Infrastructure.EntityFramework.Models;
using Infrastructure.EntityFramework.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Admin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController(PasswordService passwordService, IEventRepository eventRepository, IRepository<Guest> guestRepository) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateNewEvent([FromBody] NewEventRequestModel newEvent)
    {
        var passwordVerificationResult = passwordService.VerifyPasswordRequirements(newEvent.EventPassword);
        if (passwordVerificationResult is not null)
            return BadRequest(passwordVerificationResult);
        
        var hashedPassword = passwordService.HashPassword(newEvent.EventPassword);

        var eventEntity = new Event()
        {
            EventName = newEvent.EventName,
            EventNameShort = newEvent.EventNameShort,
            EventDate = newEvent.EventDate,
            ColorPrimary = newEvent.EventColorPrimary,
            ColorSecondary = newEvent.EventColorSecondary,
            EventStateId = newEvent.EventStateId,
            EventTypeId = newEvent.EventTypeId,
            EventPasswordHash = hashedPassword,
            DurationDays = newEvent.DurationDays,
        };
        
        var addedEvent = await eventRepository.AddAsync(eventEntity);
        
        return Ok(addedEvent);
    }

    [HttpPost("{eventId}/guests")]
    public async Task<IActionResult> InviteNewGuest([FromRoute] int eventId, [FromBody] NewGuestRequestModel newGuestRequest)
    {
        if (string.IsNullOrWhiteSpace(newGuestRequest.FirstName) || string.IsNullOrWhiteSpace(newGuestRequest.LastName))
        {
            return BadRequest("First name and last name cannot be empty");
        }

        var newGuest = new Guest()
        {
            EventId = eventId,
            FirstName = newGuestRequest.FirstName,
            LastName = newGuestRequest.LastName,
            FullName = string.Concat(newGuestRequest.FirstName, " ", newGuestRequest.LastName),  
        };
        
        return Ok(await guestRepository.AddAsync(newGuest));
    }

    [HttpGet("{eventId}/guests")]
    public async Task<IActionResult> GetGuestList([FromRoute] int eventId)
    {
        return Ok(await eventRepository.GetGuestListForEventAsync(eventId));
    }
}