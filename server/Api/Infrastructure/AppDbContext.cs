using Api.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { }

    public DbSet<Event> Events { get; set; }
    public DbSet<EventState> EventStates { get; set; }
    public DbSet<EventType> EventTypes { get; set; }
    public DbSet<Guest> Guests { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Media> Media  { get; set; }
    public DbSet<MediaType> MediaTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}