using Infrastructure.EntityFramework.Contexts;
using Infrastructure.EntityFramework.Models;
using Infrastructure.EntityFramework.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFramework.Repositories;

public class MediaRepository(AppDbContext context) : IMediaRepository
{
    public async Task<Media?> GetAsync(int id)
    {
        return await context.Media.FindAsync(id);
    }

    public async Task<IEnumerable<Media>> GetAllAsync()
    {
        return await context.Media.ToListAsync();
    }

    public IEnumerable<Media> GetAll(int eventId)
    {
        return context.Media.Where(e => e.EventId == eventId);
    }

    public IEnumerable<Media> GetAll(int eventId, bool isPrivate)
    {
        return context.Media.Where(m => m.EventId == eventId && m.IsPrivate == isPrivate);
    }

    public async Task<Media> AddAsync(Media entity)
    {
        await context.Media.AddAsync(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await context.Media.FindAsync(id);
        if  (entity is null)
            return false;   
        context.Media.Remove(entity);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<Media?> UpdateAsync(Media entity)
    {
        var mediaToUpdate = await context.Media.FindAsync(entity.Id);
        if (mediaToUpdate is null)
            return null;
        context.Entry(mediaToUpdate).CurrentValues.SetValues(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task AddMultipleAsync(IEnumerable<Media> entities)
    {
        await context.Media.AddRangeAsync(entities);
        await context.SaveChangesAsync();
    }
}