using ContentStore.Abstractions.Models;

namespace ContentStore.Abstractions.Interfaces;

/// <summary>
/// Service interface that defines interactions with a content store. 
/// </summary>
public interface IContentStoreService
{
    /// <summary>
    /// Asynchronously retrieves the list of buckets for the current content store.
    /// </summary>
    /// <returns>A <see cref="IReadOnlyCollection{StorageBucket}"/> containing the
    /// list of <see cref="StorageBucket"/> objects in the current content store</returns>
    Task<IReadOnlyCollection<StorageBucket>> ListBucketsAsync(); 
    
    /// <summary>
    /// Asynchronously creates uploads for the passed in <see cref="ContentKeyGroup"/> objects. 
    /// </summary>
    /// <param name="objects">The objects to create uploads for.</param>
    /// <returns>A <see cref="IReadOnlyCollection{string}"/> containing the uploads for the
    /// passed in <see cref="ContentKeyGroup"/> objects.</returns>
    Task<IReadOnlyCollection<string>> CreateUploadsAsync(ContentKeyGroup objects);

    /// <summary>
    /// Asynchronously creates downloads for the passed in <see cref="ContentKeyGroup"/> objects.
    /// </summary>
    /// <param name="objects">The objects to create downloads for.</param>
    /// <returns>A <see cref="IReadOnlyCollection{string}"/> containing the downloads for the
    /// passed in <see cref="ContentKeyGroup"/> objects.</returns>
    Task<IReadOnlyCollection<string>> CreateDownloadsAsync(ContentKeyGroup objects);

    /// <summary>
    /// Asynchronously deletes content specified by the passed in<see cref="ContentKeyGroup"/> objects.
    /// </summary>
    /// <param name="objects">The objects to delete.</param>
    /// <returns>A <see cref="bool"/> set to <see langword="true"/> if the deletions were a success. Otherwise <see langword="false"/>.</returns>
    Task<bool> DeleteContentAsync(ContentKeyGroup objects);
    
    /// <summary>
    /// Asynchronously deletes the content with the exact object name.
    /// </summary>
    /// <param name="objectName">The object name the delete</param>
    /// <returns>A <see cref="bool"/> set to <see langword="true"/> if the deletion was a success. Otherwise <see langword="false"/>.</returns>
    Task<bool> DeleteContentExactAsync(string objectName);
    
    /// <summary>
    /// Gets the object location in the Content Store for the object specified by the passed in <see cref="ContentKey"/>
    /// </summary>
    /// <param name="objectKey">The <see cref="ContentKey"/> specifying the object to locate.</param>
    /// <returns>The location of the object in the Content Store as a <see cref="string"/></returns>
    string GetObjectLocation(ContentKey objectKey);

    /// <summary>
    /// Asynchronously gets the names of the content in a Content Store at the specified location.
    /// </summary>
    /// <param name="location">The location specified as a <see cref="string"/>.</param>
    /// <returns>An <see cref="IReadOnlyCollection{string}"/> of names of content that reside as the specified location.</returns>
    Task<IReadOnlyCollection<string>> GetContentNamesAsync(string location);
}