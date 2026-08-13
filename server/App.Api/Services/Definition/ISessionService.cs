namespace App.Api.Services.Definition;

/// <summary>
/// Service interface for defining operations related to handling user sessions.
/// </summary>
public interface ISessionService
{
    /// <summary>
    /// Determines whether a user session with the provided Id for the provided event exists and is valid.
    /// </summary>
    /// <param name="sessionId">The unique identifier of the session.</param>
    /// <param name="eventId">The unique identifier of the event.</param>
    /// <returns>
    /// The tuple whose first value is the unique identifier of the validated session,
    /// and second value indicating when the session expires (in UTC), if the session is valid, null otherwise.</returns>
    Task<(Guid, DateTimeOffset)?> GetValidSessionAsync(Guid sessionId, Guid eventId);
    
    /// <summary>
    /// Creates a new session for the provided event.
    /// </summary>
    /// <param name="eventId">The unique identifier of the event to create the session for.</param>
    /// <returns>The tuple whose first value is the unique identifier of the newly created session, and second value indicating when the session expires (in UTC).</returns>
    Task<(Guid, DateTimeOffset)> CreateSessionAsync(Guid eventId);
}