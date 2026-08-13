using App.Api.Models.Response;

namespace App.Api.Services.Definition;

/// <summary>
/// Service interface defining functions for performing event-related operations.
/// </summary>
public interface IEventService
{
    /// <summary>
    /// Retrieves the event password hash for the event associated with the provided event public ID.
    /// </summary>
    /// <param name="eventPublicId">The public ID of the event to retrieve the password hash for.</param>
    /// <returns>
    /// The event hash if the event exists, <see langword="null"/> otherwise.
    /// </returns>
    Task<string?> GetEventPasswordHashAsync(Guid eventPublicId);
    
    /// <summary>
    /// Retrieves the event overview information for the provided event public ID.
    /// </summary>
    /// <param name="eventPublicId">The public ID of the event to retrieve the overview information for.</param>
    /// <returns>
    /// The <see cref="EventOverviewResponseModel"/> containing the overview information, <see langword="null"/> if the event doesn't exist.
    /// </returns>
    Task<EventOverviewResponseModel?> GetEventOverviewAsync(Guid eventPublicId);
    
    /// <summary>
    /// Retrieves the internal identifier for an event's public identifier.
    /// </summary>
    /// <param name="eventPublicId">The public identifier of the event.</param>
    /// <returns>The internal identifier of the event if it exists, 0 otherwise.</returns>
    Task<long> GetEventInternalIdAsync(Guid eventPublicId);
}