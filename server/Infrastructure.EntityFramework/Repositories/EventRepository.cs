using Infrastructure.EntityFramework.Contexts;
using Infrastructure.EntityFramework.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFramework.Repositories;

public class EventRepository(AppDbContext context): IRepository<Event>
{
    public async Task<Event?> GetAsync(int id)
    {
        return await context.Events.FindAsync(id);
    }

    public async Task<IEnumerable<Event>> GetAllAsync()
    {
        return await context.Events.ToListAsync();
    }

    public async Task<Event> AddAsync(Event entity)
    {
        await context.Events.AddAsync(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var eventToDelete = await context.Events.FindAsync(id);
        if (eventToDelete is null)
            return false;
        context.Events.Remove(eventToDelete);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<Event?> UpdateAsync(Event entity)
    {
        var eventToUpdate = await context.Events.FindAsync(entity.Id);
        if (eventToUpdate is null)
            return null;
        context.Entry(eventToUpdate).CurrentValues.SetValues(entity);
        await context.SaveChangesAsync();
        return entity;
    }
}