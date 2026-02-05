using Infrastructure.EntityFramework.Models;

namespace Infrastructure.EntityFramework.Repositories.Interfaces;

public interface IMediaRepository : IRepository<Media>
{
    public Task AddMultipleAsync(IEnumerable<Media> entities);
    
    public IEnumerable<Media> GetAll(int eventId);
    
    public IEnumerable<Media> GetAll(int eventId, bool isPrivate);
}