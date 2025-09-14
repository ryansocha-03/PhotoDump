using Microsoft.EntityFrameworkCore;
using photodump_api.Domain;

namespace photodump_api.Infrastructure.Db;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<Event> Events { get; set; }
    public DbSet<Guest> Guests { get; set; }
    public DbSet<Media> MediaData { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly); 
        base.OnModelCreating(modelBuilder);
    }
}