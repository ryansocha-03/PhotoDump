using Domain.Entities;
using Infrastructure.EntityFramework.Contexts;
using Microsoft.EntityFrameworkCore;
using Persistence.Abstractions.Interfaces.Repositories;

namespace Infrastructure.EntityFramework.Repositories;

/// <summary>
/// Repository implementation for interactions with <see cref="EventSession"/> entities using EF Core.
/// </summary>
public class EventSessionRepository(AppDbContext context): IEventSessionRepository 
{
    /// <inheritdoc /> 
    public async Task<EventSession?> GetAsync(Guid id)
    {
        return await context.EventSessions.FindAsync(id);
    }
    
    /// <inheritdoc />
    public async Task<EventSession?> GetValidAsync(Guid sessionId, Guid eventId)
    {
        return await context.EventSessions
            .Where(es => es.Id == sessionId 
                && es.EventPublicId == eventId
                && es.RevokedAt == null
                && es.ExpiresAt > DateTime.UtcNow)
            .FirstOrDefaultAsync();
    }
    
    /// <inheritdoc />
    public async Task<EventSession> CreateAsync(EventSession eventSession)
    {
        var resolvedEventSession = await context.EventSessions.AddAsync(eventSession);
        await context.SaveChangesAsync();
        return resolvedEventSession.Entity;
    }
    
    /// <inheritdoc />
    public async Task<EventSession?> UpdateAsync(EventSession eventSession)
    {
        var rowsUpdated = await context.EventSessions
            .Where(es => es.Id == eventSession.Id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(es => es.ExpiresAt, eventSession.ExpiresAt)
                .SetProperty(es => es.LastSeenAt, eventSession.LastSeenAt)
                .SetProperty(es => es.UserAgent, eventSession.UserAgent)
                .SetProperty(es => es.IpAddress, eventSession.IpAddress));
        
        return rowsUpdated == 0 ? null : eventSession;
    }
    
    /// <inheritdoc />
    public async Task<EventSession?> RevokeAsync(EventSession session)
    {
        var rowsUpdated = await context.EventSessions
            .Where(es => es.Id == session.Id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(es => es.RevokedAt, DateTimeOffset.UtcNow));
        
        return rowsUpdated == 0 ? null : session;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid sessionId)
    {
        var rowsDeleted = await context.EventSessions
            .Where(es => es.Id == sessionId)
            .ExecuteDeleteAsync();

        return rowsDeleted != 0;
    }

    /// <inheritdoc />
    public async Task<int> DeleteInvalidAsync()
    {
        return await context.EventSessions
            .Where(es => es.RevokedAt != null
                || es.ExpiresAt <  DateTime.UtcNow)
            .ExecuteDeleteAsync();
    }
}