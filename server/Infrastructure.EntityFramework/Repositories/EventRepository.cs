using Infrastructure.EntityFramework.Contexts;
using Microsoft.EntityFrameworkCore;
using Persistence.Abstractions.Interfaces.Repositories;
using Domain.Entities;

namespace Infrastructure.EntityFramework.Repositories;

/// <summary>
/// Repository implementation for database interactions with <see cref="Event"/> entities via EF Core.
/// </summary>
public class EventRepository(AppDbContext context): IEventRepository
{
    /// <inheritdoc />
    public async Task<Event?> GetByIdAsync(long id)
    {
        return await context.Events.FindAsync(id);
    }

    /// <inheritdoc />
    public async Task<Event?> GetByPublicIdAsync(Guid publicId)
    {
        return await context.Events.FirstOrDefaultAsync(e => e.PublicId == publicId);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Event>> GetAllAsync()
    {
        return await context.Events.ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Event> CreateAsync(Event newEvent)
    {
        var resolvedEvent = await context.Events.AddAsync(newEvent);
        await context.SaveChangesAsync();
        return resolvedEvent.Entity;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(long id)
    {
        var rowsUpdated = await context.Events
            .Where(e => e.Id == id)
            .ExecuteDeleteAsync();
        
        return rowsUpdated != 0;
    }

    /// <inheritdoc />
    public async Task<Event?> UpdateAsync(Event entity)
    {
        var rowsUpdated = await context.Events
            .Where(e => e.Id == entity.Id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(e => e.PublicId, entity.PublicId)
                .SetProperty(e => e.EventName, entity.EventName)
                .SetProperty(e => e.EventNameShort, entity.EventNameShort)
                .SetProperty(e => e.ColorPrimary, entity.ColorPrimary)
                .SetProperty(e => e.ColorSecondary, entity.ColorSecondary)
                .SetProperty(e => e.StartDate, entity.StartDate)
                .SetProperty(e => e.EndDate, entity.EndDate)
                .SetProperty(e => e.EventPasswordHash, entity.EventPasswordHash)
                .SetProperty(e => e.EventState, entity.EventState)
                .SetProperty(e => e.EventTypeId, entity.EventTypeId));
        
        return rowsUpdated == 0 ? null : entity;
    }
}