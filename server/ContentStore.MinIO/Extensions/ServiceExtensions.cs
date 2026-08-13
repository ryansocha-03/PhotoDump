using ContentStore.Abstractions.Interfaces;
using ContentStore.Abstractions.Models.Configuration;
using ContentStore.MinIO.Models.Configuration;
using ContentStore.MinIO.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ContentStore.MinIO.Extensions;

/// <summary>
/// Provides service extensions for configuring and registering a MinIO content store.
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Adds configuration options and registers services for a MinIO content store.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services and options to.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> containing the configuration values.</param>
    /// <param name="contentStoreConfigurationSection">The path to the content store configuration. Defaults to "ContentStore"</param>
    /// <param name="minIoConfigurationSection">The path to the MinIO specific configuration. Defaults to "MinIo"</param>
    public static void AddMinIoContentStore(this IServiceCollection services, IConfiguration configuration,
        string contentStoreConfigurationSection = "ContentStore", string minIoConfigurationSection = "MinIo")
    {
        services.AddOptions<MinIoConfiguration>()
            .Bind(configuration.GetSection(minIoConfigurationSection))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services.AddOptions<ContentStoreConfiguration>()
            .Bind(configuration.GetSection(contentStoreConfigurationSection))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IContentStoreService, MinioService>();
    }
}