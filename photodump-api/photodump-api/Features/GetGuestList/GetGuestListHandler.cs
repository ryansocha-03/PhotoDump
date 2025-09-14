using System.Data;
using photodump_api.Domain;
using photodump_api.Infrastructure.Db.Repositories;

namespace photodump_api.Features.GetGuestList;

public class GetGuestListHandler(IGuestRepository guestRepository)
{
    public async Task<List<Guest>> HandleAsync(int eventId)
    {
        List<Guest> guestsForEvent = await guestRepository.GetAllAsync(eventId);
        return guestsForEvent;
    }
}