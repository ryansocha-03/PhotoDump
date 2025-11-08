using Identity.Services;
using Infrastructure.EntityFramework.Models;
using Infrastructure.EntityFramework.Repositories;

namespace App.Api.Services;

public class AuthService(IRepository<Event> eventRepository, PasswordService passwordService, TokenService tokenService)
{
    public async Task<Event?> FindEventForPassword(string providedPassword)
    {
        var events = await eventRepository.GetAllAsync();
        return events.FirstOrDefault(e => passwordService.PasswordHashMatches(providedPassword, e.EventPasswordHash));
    }

    public string CreateGuestTokenForEvent(Event eventToCreate)
    {
        return $"poop{eventToCreate.EventName}";
    }
}