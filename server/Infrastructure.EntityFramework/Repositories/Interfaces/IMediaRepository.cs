using Infrastructure.EntityFramework.Models;

namespace Infrastructure.EntityFramework.Repositories.Interfaces;

public interface IMediaRepository : IRepository<Media>
{
    public Task AddMultipleAsync(IEnumerable<Media> entities);
    
    public Task<IEnumerable<Media>> GetAllAsync(int eventId);
}