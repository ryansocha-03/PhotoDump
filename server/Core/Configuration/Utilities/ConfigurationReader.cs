using Core.Configuration.ConfigurationModels;
using Core.Configuration.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Configuration.Utilities;

public static class ConfigurationReader
{
    public static void AddDatabaseConfiguration(this IServiceCollection  services, IConfiguration configuration)
    {
        services.Configure<DatabaseConfigurationModel>(configuration.GetSection("AppDatabase"));
    }

    public static void AddContentStoreConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ContentStoreConfigurationModel>(configuration.GetSection("ContentStore"));
    }

    public static void AddBrokerClientConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<BrokerClientConfigurationModel>(configuration.GetSection("BrokerClient"));
    }
}