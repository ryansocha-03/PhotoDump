using Core.Configuration.Models;
using Infrastructure.EntityFramework.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure.EntityFramework.Utilities;

public static class EFServiceExtensions
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
        
        
    }
}