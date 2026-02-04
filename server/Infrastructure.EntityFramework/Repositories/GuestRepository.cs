using Infrastructure.EntityFramework.Contexts;
using Infrastructure.EntityFramework.Models;
using Infrastructure.EntityFramework.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFramework.Repositories;

public class GuestRepository(AppDbContext context): IRepository<Guest>
{
    public async Task<Guest?> GetAsync(int id)
    {
        return await context.Guests.FindAsync(id);
    }

    public async Task<IEnumerable<Guest>> GetAllAsync()
    {
        return await context.Guests.ToListAsync();
    }

    public async Task<Guest> AddAsync(Guest entity)
    {
        await context.Guests.AddAsync(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var guestToDelete = await context.Guests.FindAsync(id);
        if (guestToDelete == null) return false;
        
        context.Guests.Remove(guestToDelete);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<Guest?> UpdateAsync(Guest entity)
    {
        var guestToUpdate = await context.Guests.FindAsync(entity.Id);
        if (guestToUpdate == null) return null;
        
        context.Entry(guestToUpdate).CurrentValues.SetValues(entity);
        await context.SaveChangesAsync();
        return guestToUpdate;
    }
}