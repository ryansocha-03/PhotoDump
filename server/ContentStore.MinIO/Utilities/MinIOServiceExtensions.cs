using ContentStore.MinIO.Services;
using Core.Configuration.ConfigurationModels;
using Core.Configuration.Models;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;

namespace ContentStore.MinIO.Utilities;

public static class MinIoServiceExtensions
{
    public static void AddMinIoServices(this IServiceCollection services, IConfiguration configuration, string environmentName)
    {
        var contentStoreConfig = configuration.GetRequiredSection("ContentStore").Get<ContentStoreConfigurationModel>();
        services.AddMinio(configureClient => configureClient
            .WithEndpoint(contentStoreConfig.Endpoint)
            .WithRegion(contentStoreConfig.Region)
            .WithCredentials(contentStoreConfig.AccessKey, contentStoreConfig.SecretKey)
            .WithSSL(environmentName.ToLower() != "development")
            .Build());
        
        services.AddSingleton<IContentStoreService, MinioService>();
    }
}