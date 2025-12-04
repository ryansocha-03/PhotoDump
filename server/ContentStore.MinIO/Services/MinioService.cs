using Core.Configuration.DTOs;
using Core.Interfaces;
using Minio;

namespace ContentStore.MinIO.Services;

public class MinioService(IMinioClient minioClient): IContentStoreService
{
    public async Task<List<StorageBucketDTO>> ListBuckets()
    {
        var buckets = await minioClient.ListBucketsAsync().ConfigureAwait(false);
        List<StorageBucketDTO> bucketDtos = [];
        bucketDtos.AddRange(buckets.Buckets.Select(bucket => new StorageBucketDTO()
        {
            BucketName = bucket.Name, 
            BucketCreationDate = DateTime.Parse(bucket.CreationDate)
        }));
        return bucketDtos;
    }
}