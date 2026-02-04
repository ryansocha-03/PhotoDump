using Infrastructure.EntityFramework.Contexts;
using Infrastructure.EntityFramework.Models;
using Infrastructure.EntityFramework.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFramework.Repositories;

public class MediaTypeRepository(AppDbContext context) : IRepository<MediaType>
{
    public async Task<MediaType?> GetAsync(int id)
    {
        return await context.MediaTypes.FindAsync(id);
    }

    public async Task<IEnumerable<MediaType>> GetAllAsync()
    {
        return await context.MediaTypes.ToListAsync();
    }

    public async Task<MediaType> AddAsync(MediaType entity)
    {
        await context.MediaTypes.AddAsync(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var mediaType = await context.MediaTypes.FindAsync(id);
        if (mediaType is null)
            return false;
        context.MediaTypes.Remove(mediaType);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<MediaType?> UpdateAsync(MediaType entity)
    {
        var mediaTypeToUpdate = await context.MediaTypes.FindAsync(entity.Id);
        if (mediaTypeToUpdate is null)
            return null;
        context.Entry(mediaTypeToUpdate).CurrentValues.SetValues(entity);
        await context.SaveChangesAsync();
        return entity;
    }
}