using Infrastructure.EntityFramework.Models;

namespace Infrastructure.EntityFramework.Repositories;

public interface IEventSessionRepository
{
    // Functionality
    Task<EventSession?> ValidateSessionAsync(Guid sessionId, Guid eventId);
    Task<EventSession> RevokeSessionAsync(EventSession session);
    
    // Primitives
    Task<EventSession?> GetAsync(Guid id);
    Task<EventSession> CreateAsync(EventSession newSession);
    Task<bool> DeleteAsync(Guid id);
    Task<EventSession?> UpdateAsync(EventSession updatedSession);
}