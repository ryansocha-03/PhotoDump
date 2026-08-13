using App.Api.Models.Response;
using App.Api.Services.Definition;
using Persistence.Abstractions.Interfaces.Repositories;

namespace App.Api.Services.Implementation;

/// <summary>
/// Service implementation for functions for performing event-related operations.
/// </summary>
public class EventService(IEventRepository eventRepository) : IEventService
{
    /// <inheritdoc />
    public async Task<string?> GetEventPasswordHashAsync(Guid eventPublicId)
    {
        return (await eventRepository.GetByPublicIdAsync(eventPublicId))?.EventPasswordHash;
    }

    /// <inheritdoc />
    public async Task<EventOverviewResponseModel?> GetEventOverviewAsync(Guid eventPublicId)
    {
        var eventDetails = await eventRepository.GetByPublicIdAsync(eventPublicId);

        return eventDetails == null
            ? null
            : new EventOverviewResponseModel
            {
                EventPublicId = eventDetails.PublicId,
                EventName = eventDetails.EventName,
                EventNameShort = eventDetails.EventNameShort,
                ColorPrimary = eventDetails.ColorPrimary,
                ColorSecondary = eventDetails.ColorSecondary,
            };
    }
    
    /// <inheritdoc />
    public async Task<long> GetEventInternalIdAsync(Guid eventPublicId)
    {
        return (await eventRepository.GetByPublicIdAsync(eventPublicId))?.Id ?? 0;
    }
}