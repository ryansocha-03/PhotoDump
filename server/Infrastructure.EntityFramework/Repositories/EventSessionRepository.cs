using Infrastructure.EntityFramework.Contexts;
using Infrastructure.EntityFramework.Models;
using Infrastructure.EntityFramework.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFramework.Repositories;

public class EventSessionRepository(AppDbContext context): IEventSessionRepository 
{
    public async Task<EventSession?> ValidateSessionAsync(Guid sessionId, Guid eventId)
    {
        return await context.EventSessions
            .Where(es => es.Id == sessionId
                && es.EventPublicId == eventId
                && es.RevokedAt == null
                && es.ExpiresAt > DateTimeOffset.UtcNow)
            .FirstOrDefaultAsync();
    }

    public async Task<EventSession> RevokeSessionAsync(EventSession session)
    {
        session.RevokedAt = DateTimeOffset.UtcNow;
        await UpdateAsync(session);
        return session;
    }

    public async Task<EventSession?> GetAsync(Guid id)
    {
        return await context.EventSessions.FindAsync(id);
    }

    public async Task<EventSession> CreateAsync(EventSession newSession)
    {
        await context.EventSessions.AddAsync(newSession);
        await context.SaveChangesAsync();
        return newSession;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var session = await GetAsync(id);
        if (session == null)
            return false;
        context.EventSessions.Remove(session);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<EventSession?> UpdateAsync(EventSession updatedSession)
    {
        var session = await GetAsync(updatedSession.Id);
        if (session == null)
            return null;
        context.Entry(session).CurrentValues.SetValues(updatedSession);
        await context.SaveChangesAsync();
        return session;
    }
}