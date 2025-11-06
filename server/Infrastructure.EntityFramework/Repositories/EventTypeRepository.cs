using Infrastructure.EntityFramework.Contexts;
using Infrastructure.EntityFramework.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFramework.Repositories;

public class EventTypeRepository(AppDbContext context) : IRepository<EventType>
{
    public async Task<EventType?> GetAsync(int id)
    {
        return await context.EventTypes.FindAsync(id);
    }

    public async Task<IEnumerable<EventType>> GetAllAsync()
    {
        return await context.EventTypes.ToListAsync();
    }

    public async Task<EventType> AddAsync(EventType entity)
    {
        await context.EventTypes.AddAsync(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entityToDelete = await context.EventTypes.FindAsync(id);
        if (entityToDelete is null)
            return false;
        

        context.EventTypes.Remove(entityToDelete);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<EventType?> UpdateAsync(EventType entity)
    {
        var entityToUpdate = await context.EventTypes.FindAsync(entity.Id);
        if (entityToUpdate is null)
            return null;    
        
        context.Entry(entityToUpdate).CurrentValues.SetValues(entity);
        await context.SaveChangesAsync();
        return entityToUpdate;
    }
}