using Infrastructure.EntityFramework.Contexts;
using Infrastructure.EntityFramework.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFramework.Repositories;

public class EventStateRepository(AppDbContext context) : IRepository<EventState>
{
    public async Task<EventState?> GetAsync(int id)
    {
        return await context.EventStates.FindAsync(id);
    }

    public async Task<IEnumerable<EventState>> GetAllAsync()
    {
        return await context.EventStates.ToListAsync();
    }

    public async Task<EventState> AddAsync(EventState entity)
    {
        await context.EventStates.AddAsync(entity);
        await  context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var eventStateToDelete = await GetAsync(id);
        if (eventStateToDelete is null)
            return false;
        
        context.EventStates.Remove(eventStateToDelete);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<EventState?> UpdateAsync(EventState entity)
    {
        var eventStateUpdated = await GetAsync(entity.Id);
        if (eventStateUpdated is null)
            return null;
        
        context.Entry(eventStateUpdated).CurrentValues.SetValues(entity);
        await context.SaveChangesAsync();
        return eventStateUpdated;
    }
}