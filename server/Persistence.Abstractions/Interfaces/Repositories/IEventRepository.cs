using Domain.Entities;

namespace Persistence.Abstractions.Interfaces.Repositories;

/// <summary>
/// Repository interface that defines interactions with <see cref="Event"/> entities.
/// </summary>
public interface IEventRepository
{
    /// <summary>
    /// Gets an <see cref="Event"/> by its internal identifier.
    /// </summary>
    /// <param name="id">The internal identifier of the <see cref="Event"/>.</param>
    /// <returns>The <see cref="Event"/> with the specified ID. <see langword="null"/> if not found.</returns>
    Task<Event?> GetByIdAsync(long id);
    
    /// <summary>
    /// Gets an <see cref="Event"/> by its public identifier.
    /// </summary>
    /// <param name="publicId">The public identifier of the <see cref="Event"/>.</param>
    /// <returns>The <see cref="Event"/> with the specified ID. <see langword="null"/> if not found.</returns>
    Task<Event?> GetByPublicIdAsync(Guid publicId);

    /// <summary>
    /// Gets all <see cref="Event"/> entities.
    /// </summary>
    /// <returns>A <see cref="IReadOnlyCollection{Event}"/> of all <see cref="Event"/></returns>
    Task<IReadOnlyCollection<Event>> GetAllAsync();
    
    /// <summary>
    /// Creates a new <see cref="Event"/>
    /// </summary>
    /// <param name="newEvent">The <see cref="Event"/> to create.</param>
    /// <returns>The fully resolved <see cref="Event"/> entity.</returns>
    Task<Event> CreateAsync(Event newEvent);
    
    /// <summary>
    /// Deletes the <see cref="Event"/> with the specified internal identifier.
    /// </summary>
    /// <param name="id">The internal identifier of the <see cref="Event"/> to delete.</param>
    /// <returns>A <see cref="bool"/> set to <see langword="true"/> if the deletion was successful,
    /// <see langword="false"/> otherwise</returns>
    Task<bool> DeleteAsync(long id);
    
    /// <summary>
    /// Updates an existing <see cref="Event"/> 
    /// </summary>
    /// <param name="updatedEvent">The <see cref="Event"/> used to identify the record to update and
    /// that contains the updated values.</param>
    /// <returns>The fully resolved <see cref="Event"/>entity after update, <see langwod="null"/> if not found.</returns>
    Task<Event?> UpdateAsync(Event updatedEvent);
}