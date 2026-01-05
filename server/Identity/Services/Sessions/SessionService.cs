using Identity.Models;
using Infrastructure.EntityFramework.Models;
using Infrastructure.EntityFramework.Repositories;

namespace Identity.Services.Sessions;

public class SessionService(IEventSessionRepository  repository)
{
    public async Task<bool> ValidateSessionAsync(Guid sessionId, Guid eventId)
    {
        var session = await repository.ValidateSessionAsync(sessionId, eventId);
        return session != null;
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
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(SessionConfiguration.SessionDuration),
            LastSeenAt = DateTimeOffset.UtcNow
        };
        
        await repository.CreateAsync(eventSession);

        return (eventSession.Id, eventSession.ExpiresAt);
    }
}