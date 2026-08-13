namespace Persistence.Abstractions.Interfaces.Repositories;
using Domain.Entities;

/// <summary>
/// Repository interface that defines interactions with <see cref="EventType"/> entities.
/// </summary>
public interface IEventTypeRepository
{
    /// <summary>
    /// Gets all <see cref="EventType"/> entities.
    /// </summary>
    /// <returns>A <see cref="IReadOnlyCollection{EventType}"/> of all <see cref="EventType"/> entities.</returns>
    Task<IReadOnlyCollection<EventType>> GetAllAsync();
    
    /// <summary>
    /// Gets a <see cref="EventType"/> by its internal identifier.
    /// </summary>
    /// <param name="id">The internal identifier of the <see cref="EventType"/></param>
    /// <returns>The <see cref="EventType"/> with the specified ID. <see langword="null"/> if not found.</returns>
    Task<EventType?> GetByIdAsync(long id);
    
    /// <summary>
    /// Creates a new <see cref="EventType"/>
    /// </summary>
    /// <param name="newEventType">The <see cref="EventType"/> to create.</param>
    /// <returns>The fully resolved <see cref="EventType"/> entity.</returns>
    Task<EventType> CreateAsync(EventType newEventType);
    
    /// <summary>
    /// Updates an existing <see cref="EventType"/>
    /// </summary>
    /// <param name="updatedEvent">The <see cref="EventType"/> used to identify the record to update, and that contains the updated values.</param>
    /// <returns>The fully resolved <see cref="EventType"/> entity after update, <see langword="null"/> if not found.</returns>
    Task<EventType?> UpdateAsync(EventType updatedEvent);
    
    /// <summary>
    /// Deletes an <see cref="EventType"/>
    /// </summary>
    /// <param name="id">The unique identifier of the <see cref="EventType"/> to delete.</param>
    /// <returns>A <see cref="bool"/> set to <see langword="true"/> if the deletion was successful, otherwise <see langword="false"/></returns>
    Task<bool> DeleteAsync(long id);
}