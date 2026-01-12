using Core.Configuration.ConfigurationModels;
using Core.Configuration.DTOs;
using Core.Interfaces;
using Core.Models;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace ContentStore.MinIO.Services;

public class MinioService(IMinioClient minioClient, IOptions<ContentStoreConfigurationModel> options): IContentStoreService
{
    public async Task<List<StorageBucketDto>> ListBuckets()
    {
        var buckets = await minioClient.ListBucketsAsync();
        List<StorageBucketDto> bucketDtos = [];
        bucketDtos.AddRange(buckets.Buckets.Select(bucket => new StorageBucketDto()
        {
            BucketName = bucket.Name, 
            BucketCreationDate = DateTime.Parse(bucket.CreationDate)
        }));
        return bucketDtos;
    }

    public async Task<string?> GeneratePresignedDownloadUrl(Guid eventId, FilePrivacyEnum privacy)
    {
        try
        {
            var args = new PresignedGetObjectArgs()
                .WithBucket(options.Value.Bucket)
                .WithObject($"{eventId}/{privacy.ToString().ToLower()}")
                .WithExpiry(options.Value.PresignedDownloadDurationMinutes * 60);

            var url = await minioClient.PresignedGetObjectAsync(args);
            return url;
        }
        catch (MinioException e)
        {
            return null;
        }
    }

    public async Task<string?> GeneratePresignedUploadUrl(Guid eventId, FilePrivacyEnum privacy)
    {
        try
        {
            var args = new PresignedPutObjectArgs()
                .WithBucket(options.Value.Bucket)
                .WithObject($"{eventId}/{privacy.ToString().ToLower()}")
                .WithExpiry(options.Value.PresignedUploadDurationMinutes * 60);

            return await minioClient.PresignedPutObjectAsync(args);
        }
        catch (MinioException e)
        {
            return null;
        }
    }
}