using Core.Interfaces;
using Minio;

namespace ContentStore.MinIO.Services;

public class MinioService(IMinioClient minioClient): IContentStoreService
{
    public async Task ListBuckets()
    {
        var buckets = await minioClient.ListBucketsAsync().ConfigureAwait(false);
        foreach (var bucket in buckets.Buckets) Console.WriteLine($"{bucket.Name} {bucket.CreationDateDateTime}");
        Console.WriteLine();
    }
}