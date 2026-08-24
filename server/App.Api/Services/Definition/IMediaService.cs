using App.Api.Models.DTOs;
using App.Api.Models.Request;
using Domain.Enums;

namespace App.Api.Services.Definition;

/// <summary>
/// Service interface defining functions dealing with Media operations.
/// </summary>
public interface IMediaService
{
    /// <summary>
    /// Retrieves the information necessary for building the canonical public names of Media as a <see cref="PaginationDto{T}"/>
    /// of <see cref="string"/> for the provided event and pagination parameters.
    /// </summary>
    /// <param name="eventInternalId">The internal identifier of the event.</param>
    /// <param name="eventPublicId">The public identifier of the event (used for building cursor)</param>
    /// <param name="encodedCursor">The cursor of where to start retrieving items from.</param>
    /// <param name="limit">The max number of items to return in the <see cref="PaginationDto{T}"/></param>
    /// <returns>A <see cref="PaginationDto{T}"/> with the event's public media public file names.</returns>
    Task<PaginationDto<string>> GetEventPublicMediaPagedAsync(long eventInternalId, Guid eventPublicId, string? encodedCursor, int? limit);

    /// <summary>
    /// Creates new media entries for the provided event, privacy settings, and upload data. 
    /// </summary>
    /// <param name="eventInternalId">The internal identifier of the event the media is associated with.</param>
    /// <param name="privacy">The <see cref="FilePrivacyEnum"/> specifying the privacy of the new uploads.</param>
    /// <param name="mediaUploadInfos">The list of <see cref="MediaUploadInfo"/> containing the individual new media data.</param>
    /// <returns>A list of <see langword="string"/> of the public-facing file names of the newly created media.</returns>
    /// <exception cref="ArgumentException">If one of the media in the batch is invalid.</exception>
    Task<List<string>> CreateNewMediaEntriesAsync(long eventInternalId, FilePrivacyEnum privacy, List<MediaUploadInfo> mediaUploadInfos);

    /// <summary>
    /// Acknowledges a upload of a piece of media by its public event and file identifiers.
    /// </summary>
    /// <param name="publicFileName">The public file name of the media uploaded.</param>
    /// <param name="eventPublicId">The public identifier of the event the media is associated with.</param>
    /// <returns>A <see cref="MediaStateTransitionDto"/> containing the media information if the state transition was successful, <see langword="null"/> otherwise.</returns>
    /// <exception cref="InvalidOperationException">If the media state transition affected an invalid number of entities.</exception>
    Task<MediaStateTransitionDto?> AcknowledgeMediaUploadAsync(string publicFileName, Guid eventPublicId);
    
    /// <summary>
    /// Acknowledges the completion of the processing pipeline of a particular media entity. 
    /// </summary>
    /// <param name="mediaInternalId">The internal identifier of the media that was processed.</param>
    /// <returns>A <see cref="MediaStateTransitionDto"/> containing the media information if the state transition was successful, <see langword="null"/> otherwise.</returns>
    /// <exception cref="InvalidOperationException">If the media state transition affected an invlid number of entities.</exception>
    Task<MediaStateTransitionDto?> AcknowledgeMediaProcessingCompletionAsync(long mediaInternalId);
}