using Domain.Entities;
using Domain.Enums;

namespace Persistence.Abstractions.Interfaces.Repositories;

/// <summary>
/// Repository interface defining interactions with <see cref="Media"/> entities.
/// </summary>
public interface IMediaRepository
{
    /// <summary>
    /// Gets all <see cref="Media"/> entities.
    /// </summary>
    /// <returns>A <see cref="IReadOnlyCollection{Media}"/> of all <see cref="Media"/> entities.</returns>
    Task<IReadOnlyCollection<Media>> GetAllAsync();
    
    /// <summary>
    /// Gets all <see cref="Media"/> entities for the specified <see cref="Event"/> identifier.
    /// </summary>
    /// <param name="eventId">The internal identifier of the <see cref="Event"/> to retrieve <see cref="Media"/> for.</param>
    /// <returns>A <see cref="IReadOnlyCollection{Media}"/> containing the <see cref="Media"/> for the specified <see cref="Event"/></returns>
    Task<IReadOnlyCollection<Media>> GetAllForEventAsync(long eventId);
    
    /// <summary>
    /// Gets <see cref="Media"/> entities of the specified <see cref="FilePrivacyEnum"/> for the specified <see cref="Event"/> identifier.
    /// </summary>
    /// <param name="eventId">The internal identifier of the <see cref="Event"/> to retrieve <see cref="Media"/> for.</param>
    /// <param name="privacy">The <see cref="FilePrivacyEnum"/> specifying the privacy level of the returned <see cref="Media"/></param>
    /// <returns>A <see cref="IReadOnlyCollection{Media}"/> containing the <see cref="Media"/> for the
    ///  specified <see cref="Event"/> and <see cref="FilePrivacyEnum"/></returns>
    Task<IReadOnlyCollection<Media>> GetAllForEventAsync(long eventId, FilePrivacyEnum privacy);
    
    /// <summary>
    /// Gets a <see cref="Media"/> by its internal identifier.
    /// </summary>
    /// <param name="id">The internal identifier of the <see cref="Media"/> entity.</param>
    /// <returns>The <see cref="Media"/> with the specified internal identifier. <see langword="null"/> if not found.</returns>
    Task<Media?> GetByIdAsync(long id);

    /// <summary>
    /// Gets a <see cref="Media"/> by its public file name and associated event.
    /// </summary>
    /// <param name="fileName">The public file name.</param>
    /// <param name="eventId">The internal identifier of the <see cref="Event"/> the <see cref="Media"/> is associate with.</param>
    /// <returns>The <see cref="Media"/> entity if found, <see langword="null"/> otherwise.</returns>
    Task<Media?> GetByFileNameAndEventAsync(string fileName, long eventId);
    
    /// <summary>
    /// Gets a collection of <see cref="Media"/> given the provided event, privacy, and paging parameters.
    /// </summary>
    /// <param name="eventId">The internal identifier of the <see cref="Event"/> to get <see cref="Media"/> for.</param>
    /// <param name="privacy">The <see cref="FilePrivacyEnum"/> of the <see cref="Media"/> to fetch.</param>
    /// <param name="status">The <see cref="ContentStatusEnum"/> of the <see cref="Media"/> to fetch.</param>
    /// <param name="limit">The quantity of data to fetch.</param>
    /// <param name="cursor">The cursor indicating where to start reading data from.</param>
    /// <returns></returns>
    Task<IEnumerable<Media>> GetMediaPagedAsync(long eventId, FilePrivacyEnum privacy, ContentStatusEnum status, int limit, long? cursor);
    
    /// <summary>
    /// Creates a new <see cref="Media"/> entity.
    /// </summary>
    /// <param name="entity">The new <see cref="Media"/> entity to create.</param>
    /// <returns>The fully resolved <see cref="Media"/> entity.</returns>
    Task<Media> CreateAsync(Media entity);
    
    /// <summary>
    /// Bulk creates new <see cref="Media"/> from the provided list of entities.
    /// </summary>T
    /// <param name="entities">The list of <see cref="Media"/> entities to create.</param>
    /// <returns>The fully resolved <see cref="Media"/> entities.</returns>
    Task<IEnumerable<Media>> CreateBulkAsync(IEnumerable<Media> entities);
    
    /// <summary>
    /// Updates an existing <see cref="Media"/> entity.
    /// </summary>
    /// <param name="updatedMedia">The <see cref="Media"/> used to identify the record to update, and that contains the updated values.</param>
    /// <returns>The fully resolved <see cref="Media"/> entity after update, or <see langword="null"/> if not found.</returns>
    Task<Media?> UpdateAsync(Media updatedMedia);

    /// <summary>
    /// Updates a <see cref="Media"/> entity's state based on its internal identifier.
    /// </summary>
    /// <param name="mediaId">The internal identifier of the <see cref="Media"/></param>
    /// <param name="currentStatus">The current <see cref="ContentStatusEnum"/> of the <see cref="Media"/></param>
    /// <param name="desiredStatus">The desired <see cref="ContentStatusEnum"/> of the <see cref="Media"/></param>
    /// <returns>The fully resolved <see cref="Media"/> entity after update, or <see landword="null"/> if not found.</returns>
    Task<Media?> UpdateMediaStateByIdAsync(long mediaId, ContentStatusEnum currentStatus,
        ContentStatusEnum desiredStatus);
    
    /// <summary>
    /// Updates a <see cref="Media"/> entity's state based on its public file name and event.
    /// </summary>
    /// <param name="fileName">The public file name of the <see cref="Media"/></param>
    /// <param name="publicEventId">The public identifier of the <see cref="Event"/> associated with the <see cref="Media"/></param>
    /// <param name="currentStatus">The current <see cref="ContentStatusEnum"/> of the <see cref="Media"/></param>
    /// <param name="desiredStatus">The desired <see cref="ContentStatusEnum"/> of the <see cref="Media"/></param>
    /// <returns>The fully resolved <see cref="Media"/> entity after update, or <see langword="null"/> if not found.</returns>
    Task<Media?> UpdateMediaStateByNameAsync(string fileName, Guid publicEventId, ContentStatusEnum currentStatus, 
        ContentStatusEnum desiredStatus);
    
    /// <summary>
    /// Deletes a <see cref="Media"/> entity.
    /// </summary>
    /// <param name="id">The internal identifier of the <see cref="Media"/> entity to delete.</param>
    /// <returns>A <see cref="bool"/> set to <see langword="true"/> if the deletion was successful, <see langword="false"/> otherwise.</returns>
    Task<bool> DeleteAsync(long id);
}