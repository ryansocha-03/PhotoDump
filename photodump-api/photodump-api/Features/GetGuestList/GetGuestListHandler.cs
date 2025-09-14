using System.Data;
using photodump_api.Domain;
using photodump_api.Infrastructure.Db.Repositories;
using photodump_api.Shared.Interfaces;

namespace photodump_api.Features.GetGuestList;

public class GetGuestListHandler(IGuestRepository guestRepository): IFeatureHandler<int, List<Guest>>
{
    public async Task<List<Guest>> HandleAsync(int eventId)
    {
        List<Guest> guestsForEvent = await guestRepository.GetAllAsync(eventId);
        return guestsForEvent;
    }
}