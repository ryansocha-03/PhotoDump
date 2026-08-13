using App.Api.Models.DTOs;

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
}