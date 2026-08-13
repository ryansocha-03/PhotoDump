using App.Api.Models.Configuration;
using App.Api.Services.Definition;
using Domain.Entities;
using Microsoft.Extensions.Options;
using Persistence.Abstractions.Interfaces.Repositories;

namespace App.Api.Services.Implementation;

/// <summary>
/// Service implementation for operations relating to sessions.
/// </summary>
public class SessionService(IEventSessionRepository eventSessionRepository, IOptions<SessionAuthConfiguration> sessionAuthOptions) : ISessionService
{
    /// <inheritdoc />
    public async Task<(Guid, DateTimeOffset)?> GetValidSessionAsync(Guid sessionId, Guid eventId)
    {
        var sessionDetails = await eventSessionRepository.GetValidAsync(sessionId, eventId);
        
        return sessionDetails != null ? (sessionDetails.Id, sessionDetails.ExpiresAt) : null;
    }
    
    /// <inheritdoc />
    public async Task<(Guid, DateTimeOffset)> CreateSessionAsync(Guid eventId)
    {
        var initialEventSession = new EventSession
        {
            EventPublicId = eventId,
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(sessionAuthOptions.Value.SessionDurationMinutes),
            LastSeenAt = DateTimeOffset.UtcNow
        };

        var resolvedEventSession = await eventSessionRepository.CreateAsync(initialEventSession);

        return (resolvedEventSession.Id, resolvedEventSession.ExpiresAt);
    }
}