using Core.Configuration.Models;
using Infrastructure.EntityFramework.Contexts;
using Infrastructure.EntityFramework.Models;
using Infrastructure.EntityFramework.Repositories;
using Infrastructure.EntityFramework.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure.EntityFramework.Utilities;

public static class EfServiceExtensions
{
    public static void AddDatabaseRepositories(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>((provider, options) =>
        {
            var databaseConfiguration = provider.GetRequiredService<IOptions<DatabaseConfigurationModel>>().Value;
            switch (databaseConfiguration.DatabaseProvider)
            {
                case "Postgres":
                    options.UseNpgsql(databaseConfiguration.ConnectionString);
                    break;
                default:
                    options.UseNpgsql(databaseConfiguration.ConnectionString);
                    break;
            }
        });

        services.AddScoped<IRepository<EventType>, EventTypeRepository>();
        services.AddScoped<IRepository<EventState>, EventStateRepository>();
        services.AddScoped<IRepository<MediaType>, MediaTypeRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IRepository<Guest>, GuestRepository>();
        services.AddScoped<IEventSessionRepository, EventSessionRepository>();
        services.AddScoped<IMediaRepository, MediaRepository>();
    }
    
}