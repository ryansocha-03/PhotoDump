using Infrastructure.EntityFramework.Models;

namespace Infrastructure.EntityFramework.Repositories;

public class MediaRepository : IMediaRepository
{
    public Task<Media?> GetAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Media>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Media> AddAsync(Media entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Media?> UpdateAsync(Media entity)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Media>> AddMultipleAsync(IEnumerable<Media> entities)
    {
        throw new NotImplementedException();
    }
}