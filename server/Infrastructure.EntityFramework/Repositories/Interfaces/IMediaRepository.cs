using Infrastructure.EntityFramework.Models;

namespace Infrastructure.EntityFramework.Repositories;

public interface IMediaRepository : IRepository<Media>
{
    public Task<IEnumerable<Media>> AddMultipleAsync(IEnumerable<Media> entities);
}