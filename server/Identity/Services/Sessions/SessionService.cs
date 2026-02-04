using Identity.Models;
using Infrastructure.EntityFramework.Models;
using Infrastructure.EntityFramework.Repositories;
using Infrastructure.EntityFramework.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace Identity.Services.Sessions;

public class SessionService(IEventSessionRepository  repository, 
    IEventRepository eventRepository,
    IRepository<Guest> guestRepository, 
    IOptions<SessionAuthConfigurationModel> configuration)
{
    public async Task<bool> ValidateSessionAsync(Guid sessionId, Guid eventId)
    {
        var session = await repository.ValidateSessionAsync(sessionId, eventId);
        return session != null;
    }

    public async Task<bool> UpgradeSessionToGuestAsync(Guid sessionId, Guid eventId, int guestId)
    {
        var session = await repository.GetAsync(sessionId);
        if (session == null)
            return false;
        
        var eventData = await eventRepository.GetByPublicIdAsync(eventId);
        if (eventData == null)
            return false;
        
        var guest = await guestRepository.GetAsync(guestId);
        if (guest == null)
            return false;

        if (guest.EventId != eventData.Id)
            return false;
        
        session.GuestId = guestId;
        await repository.UpdateAsync(session);
        return true;
    }

    public async Task<SessionType?> GetSessionTypeAsync(Guid sessionId, Guid eventId)
    {
        var session = await repository.ValidateSessionAsync(sessionId, eventId);
        if (session == null)
            return null;
        return session.GuestId != null ? SessionType.Guest : SessionType.Anonymous;
    }

    public async Task<(Guid, DateTimeOffset)> CreateSessionAsync(Guid eventId)
    {
        var eventSession = new EventSession()
        {
            EventPublicId = eventId,
            CreatedAt = DateTimeOffset.UtcNow,
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(configuration.Value.SessionDurationMinutes),
            LastSeenAt = DateTimeOffset.UtcNow
        };
        
        await repository.CreateAsync(eventSession);

        return (eventSession.Id, eventSession.ExpiresAt);
    }
}