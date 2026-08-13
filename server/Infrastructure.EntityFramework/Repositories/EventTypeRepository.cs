using Domain.Entities;
using Infrastructure.EntityFramework.Contexts;
using Microsoft.EntityFrameworkCore;
using Persistence.Abstractions.Interfaces.Repositories;

namespace Infrastructure.EntityFramework.Repositories;

/// <summary>
/// Repository implementation was interacting with <see cref="EventType"/> entities using EF Core.
/// </summary>
public class EventTypeRepository(AppDbContext context) : IEventTypeRepository
{
    /// <inheritdoc />
    public async Task<IReadOnlyCollection<EventType>> GetAllAsync()
    {
        return await context.EventTypes.ToListAsync();
    }

    /// <inheritdoc />
    public async Task<EventType?> GetByIdAsync(long id)
    {
        return await context.EventTypes.FindAsync(id);
    }

    /// <inheritdoc />
    public async Task<EventType> CreateAsync(EventType newEventType)
    {
        var resolvedEntity = await context.EventTypes.AddAsync(newEventType);
        await context.SaveChangesAsync();
        return resolvedEntity.Entity;
    }

    /// <inheritdoc />
    public async Task<EventType?> UpdateAsync(EventType updatedEvent)
    {
        var updatedRows = await context.EventTypes
            .Where(et => et.Id == updatedEvent.Id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(et => et.TypeName, updatedEvent.TypeName));
        
        return updatedRows == 0 ? null : updatedEvent;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(long id)
    {
        var deletedRows = await context.EventTypes
            .Where(et => et.Id == id)
            .ExecuteDeleteAsync();
        
        return deletedRows != 0;
    }
}