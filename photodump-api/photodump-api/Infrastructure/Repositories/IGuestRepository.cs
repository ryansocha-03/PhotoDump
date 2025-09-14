using photodump_api.Domain;

namespace photodump_api.Infrastructure.Db.Repositories;

public interface IGuestRepository
{
    public Task<List<Guest>> GetAllAsync(int eventId);
    
    public Task<Guest?> GetAsync(int eventId, int guestId);
}