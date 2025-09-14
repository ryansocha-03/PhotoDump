using Microsoft.EntityFrameworkCore;
using photodump_api.Domain;

namespace photodump_api.Infrastructure.Db.Repositories;

public class GuestRepository(AppDbContext context): IGuestRepository
{
    public async Task<List<Guest>> GetAllAsync(int eventId)
    {
        return await context.Guests
            .Where(g => g.EventId == eventId)
            .ToListAsync();
    }

    public async Task<Guest?> GetAsync(int eventId, int guestId)
    {
        return await context.Guests
            .Where(g => g.EventId == eventId && g.Id == guestId)
            .FirstOrDefaultAsync();
    }
}