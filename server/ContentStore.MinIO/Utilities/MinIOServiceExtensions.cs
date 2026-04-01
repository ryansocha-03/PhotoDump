using ContentStore.MinIO.Services;
using ContentStore.MinIO.Wrappers;
using Core.Configuration.ConfigurationModels;
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

        var externalStorageClient = new MinioClient()
            .WithEndpoint(contentStoreConfig.ExternalEndpoint)
            .WithRegion(contentStoreConfig.Region)
            .WithCredentials(contentStoreConfig.AccessKey, contentStoreConfig.SecretKey)
            .WithSSL(!environmentName.Equals("development", StringComparison.CurrentCultureIgnoreCase))
            .Build();
        
        services.AddSingleton<IExternalS3Client>(new ExternalS3Client(externalStorageClient));
        
        var internalStorageClient = new MinioClient() 
            .WithEndpoint(contentStoreConfig.InternalEndpoint)
            .WithRegion(contentStoreConfig.Region)
            .WithCredentials(contentStoreConfig.AccessKey, contentStoreConfig.SecretKey)
            .WithSSL(!environmentName.Equals("development", StringComparison.CurrentCultureIgnoreCase))
            .Build();
        
        services.AddSingleton<IInternalS3Client>(new InternalS3Client(internalStorageClient));
        
        services.AddSingleton<IContentStoreService, MinioService>();
    }
}