using Core.Configuration.DTOs;
using Core.Interfaces;
using Minio;

namespace ContentStore.MinIO.Services;

public class MinioService(IMinioClient minioClient): IContentStoreService
{
    public async Task<List<StorageBucketDto>> ListBuckets()
    {
        var buckets = await minioClient.ListBucketsAsync().ConfigureAwait(false);
        List<StorageBucketDto> bucketDtos = [];
        bucketDtos.AddRange(buckets.Buckets.Select(bucket => new StorageBucketDto()
        {
            BucketName = bucket.Name, 
            BucketCreationDate = DateTime.Parse(bucket.CreationDate)
        }));
        return bucketDtos;
    }
}