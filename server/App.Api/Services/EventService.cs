using App.Api.Models.Response;
using Infrastructure.EntityFramework.Repositories;

namespace App.Api.Services;

public class EventService(IEventRepository eventRepository)
{
    public async Task<EventLandingResponseModel?> FetchLandingDetailsAsync(Guid eventPublicId)
    {
        var eventData = await eventRepository.GetByPublicIdAsync(eventPublicId);
        if (eventData is null)
            return null;
        return new EventLandingResponseModel()
        {
            Id = eventData.Id,
            EventPublicId = eventData.PublicId,
            EventName = eventData.EventName,
            EventNameShort = string.IsNullOrWhiteSpace(eventData.EventNameShort) ? string.Empty : eventData.EventNameShort,
            ColorPrimary = string.IsNullOrWhiteSpace(eventData.ColorPrimary) ? string.Empty : eventData.ColorPrimary,
            ColorSecondary = string.IsNullOrWhiteSpace(eventData.ColorSecondary) ? string.Empty : eventData.ColorSecondary,
        };
    }

    public async Task<string?> FetchEventHashAsync(Guid eventPublicId)
    {
        var eventData = await eventRepository.GetByPublicIdAsync(eventPublicId);

        return eventData?.EventPasswordHash;
    }
}