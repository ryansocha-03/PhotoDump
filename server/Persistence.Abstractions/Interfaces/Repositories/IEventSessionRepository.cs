using Domain.Entities;

namespace Persistence.Abstractions.Interfaces.Repositories;

/// <summary>
/// Repository interface that defines interactions with <see cref="EventSession"/> entities.
/// </summary>
public interface IEventSessionRepository
{
    /// <summary>
    /// Gets the <see cref="EventSession"/> by its unique identifier.
    /// </summary>
    /// <param name="sessionId">The unique identifier of the <see cref="EventSession"/></param>
    /// <returns>The <see cref="EventSession"/> with the unique identifier if found. <see langword="null"/> otherwise</returns>
    Task<EventSession?> GetAsync(Guid sessionId);
    
    /// <summary>
    /// Gets a valid <see cref="EventSession"/> by its unique identifier and event identifier.
    /// </summary>
    /// <param name="sessionId">The unique identifier of the <see cref="EventSession"/></param>
    /// <param name="eventId">The public identifier of the <see cref="Event"/> the <see cref="EventSession"/> is for.</param>
    /// <returns>The <see cref="EventSession"/> if found an valid, <see langword="null"/> otherwise</returns>
    Task<EventSession?> GetValidAsync(Guid sessionId, Guid eventId);
    
    /// <summary>
    /// Creates a new <see cref="EventSession"/>
    /// </summary>
    /// <param name="eventSession">The <see cref="EventSession"/> to create.</param>
    /// <returns>The fully resolved <see cref="EventSession"/> entity.</returns>
    Task<EventSession> CreateAsync(EventSession eventSession);
    
    /// <summary>
    /// Updates an existing <see cref="EventSession"/> metadata.
    /// </summary>
    /// <param name="eventSession">The <see cref="EventSession"/> used to identify the record to update
    /// and that contains the updated values.</param>
    /// <returns>The fully resolved <see cref="EventSession"/> entity after the update, otherwise <see langword="null"/> if not found.</returns>
    Task<EventSession?> UpdateAsync(EventSession eventSession);
    
    /// <summary>
    /// Revokes the specified <see cref="EventSession"/>, making it no longer valid.
    /// </summary>
    /// <param name="eventSession">The <see cref="EventSession"/> used to identify the record to revoke.</param>
    /// <returns>The fully resolved <see cref="EventSession"/> after being invalidated, <see langword="null"/> if not found.</returns>
    Task<EventSession?> RevokeAsync(EventSession eventSession);
    
    /// <summary>
    /// Deletes the <see cref="EventSession"/> with the specified unique identifier.
    /// </summary>
    /// <param name="sessionId">The unique identifier of the <see cref="EventSession"/> to delete.</param>
    /// <returns>A <see cref="bool"/> set to <see langword="true"/> if the deletion was successful, <see langword="false"/> otherwise</returns>
    Task<bool> DeleteAsync(Guid sessionId);
    
    /// <summary>
    /// Deletes all invalid <see cref="EventSession"/> entities.
    /// </summary>
    /// <returns>The count of <see cref="EventSession"/> entities deleted.</returns>
    Task<int> DeleteInvalidAsync();
}