using Infrastructure.EntityFramework.Contexts;
using Infrastructure.EntityFramework.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Abstractions.Interfaces.Repositories;
using Persistence.Abstractions.Models.Configuration;

namespace Infrastructure.EntityFramework.Extensions;

/// <summary>
/// Provides service extensions for configuring and registering an application database using EF Core.
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Adds configuration options and other required services for interacting with an application database using EF Core.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to register the services with.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> that holds the database configuration</param>
    /// <param name="databaseConfigurationSection">The path to the database configuration. Defaults to 'AppDatabase'</param>
    /// <exception cref="ArgumentNullException">If the database configuration at the specified path is missing.</exception>
    public static void AddEfCoreDatabase(
        this IServiceCollection services, 
        IConfiguration configuration, 
        string databaseConfigurationSection = "AppDatabase")
    {
        var databaseConfiguration = configuration.GetSection(databaseConfigurationSection);
        services.AddOptions<DatabaseConfiguration>()
            .Bind(databaseConfiguration)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        var databaseConfigurationValue = databaseConfiguration.Get<DatabaseConfiguration>();
        if (databaseConfigurationValue == null)
            throw new ArgumentNullException(nameof(databaseConfigurationValue));
        
        services.AddDbContext<AppDbContext>((_, options) =>
        {
            switch (databaseConfigurationValue.DatabaseProvider)
            {
                default:
                    options.UseNpgsql(databaseConfigurationValue.ConnectionString);
                    break;
            }
        });
        
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IEventSessionRepository, EventSessionRepository>();
        services.AddScoped<IEventTypeRepository, EventTypeRepository>();
        services.AddScoped<IMediaRepository, MediaRepository>();
    }
    
}