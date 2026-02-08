using ContentStore.MinIO.Interfaces;
using Core.Configuration.ConfigurationModels;
using Core.Configuration.DTOs;
using Core.Interfaces;
using Core.Models;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace ContentStore.MinIO.Services;

public class MinioService(
    IExternalS3Client externalS3Client,
    IInternalS3Client internalS3Client,
    IOptions<ContentStoreConfigurationModel> options)
    : IContentStoreService
{
    private readonly IMinioClient _externalS3Client = externalS3Client.MinioClient;
    private readonly IMinioClient _internalS3Client = internalS3Client.MinioClient;
    private readonly ContentStoreConfigurationModel _contentStoreConfiguration = options.Value;

    public async Task<List<StorageBucketDto>> ListBuckets()
    {
        var buckets = await _internalS3Client.ListBucketsAsync();
        List<StorageBucketDto> bucketDtos = [];
        bucketDtos.AddRange(buckets.Buckets.Select(bucket => new StorageBucketDto()
        {
            BucketName = bucket.Name, 
            BucketCreationDate = DateTime.Parse(bucket.CreationDate)
        }));
        return bucketDtos;
    }

    public async Task<IEnumerable<string>> GenerateBulkPresignedUploadUrls(IEnumerable<string> fileNames,
        string eventId, FilePrivacyEnum privacy)
    {
        List<string> urls = []; 
        var privacyString = privacy.ToString().ToLower(); 

        var args = new PresignedPutObjectArgs()
            .WithBucket(_contentStoreConfiguration.Bucket)
            .WithExpiry(_contentStoreConfiguration.PresignedUploadDurationMinutes * 60);
        
        foreach (var fileName in fileNames)
        {
            args.WithObject($"{eventId}/{privacyString}/{fileName}");
            urls.Add(await _externalS3Client.PresignedPutObjectAsync(args));
        }

        return urls;
    }

    public async Task<IEnumerable<string>> GenerateBulkPresignedDownloadUrls(IEnumerable<string> fileNames, string eventId, FilePrivacyEnum privacy)
    {
        List<string> urls = [];
        var privacyString = privacy.ToString().ToLower();
        
        var args = new PresignedGetObjectArgs()
            .WithBucket(_contentStoreConfiguration.Bucket)
            .WithExpiry(_contentStoreConfiguration.PresignedDownloadDurationMinutes * 60);

        foreach (var fileName in fileNames)
        {
            args.WithObject($"{eventId}/{privacyString}/{fileName}");
            urls.Add(await _externalS3Client.PresignedGetObjectAsync(args));
        }
        
        return urls;
    }

    public async Task<string?> GeneratePresignedDownloadUrl(Guid eventId, FilePrivacyEnum privacy, string fileName)
    {
        try
        {
            var args = new PresignedGetObjectArgs()
                .WithBucket(_contentStoreConfiguration.Bucket)
                .WithObject($"{eventId}/{privacy.ToString().ToLower()}/{fileName}")
                .WithExpiry(_contentStoreConfiguration.PresignedDownloadDurationMinutes * 60);

            var url = await _externalS3Client.PresignedGetObjectAsync(args);
            return url;
        }
        catch (MinioException e)
        {
            return null;
        }
    }

    public async Task<string?> GeneratePresignedUploadUrl(Guid eventId, FilePrivacyEnum privacy, string fileName)
    {
        try
        {
            var args = new PresignedPutObjectArgs()
                .WithBucket(_contentStoreConfiguration.Bucket)
                .WithObject($"{eventId}/{privacy.ToString().ToLower()}/{fileName}")
                .WithExpiry(_contentStoreConfiguration.PresignedUploadDurationMinutes * 60);

            return await _externalS3Client.PresignedPutObjectAsync(args);
        }
        catch (MinioException e)
        {
            return null;
        }
    }

    public async Task<bool> DeleteMediaFromEvent(Guid eventId, FilePrivacyEnum privacy, string fileName)
    {
        var args = new RemoveObjectArgs()
            .WithBucket(_contentStoreConfiguration.Bucket)
            .WithObject($"{eventId}/{privacy.ToString().ToLower()}/{fileName}");

        try
        {
            await _internalS3Client.RemoveObjectAsync(args);
        }
        catch
        {
            return false;
        }

        return true;
    }

    public async Task<List<string>> ListObjectsInBucket(Guid eventId)
    {
        var args = new ListObjectsArgs()
            .WithBucket(_contentStoreConfiguration.Bucket)
            .WithRecursive(true)
            .WithPrefix(eventId.ToString());

        var objects = _internalS3Client.ListObjectsEnumAsync(args);
        
        var items = new List<string>();
        await foreach (var obj in objects)
        {
            items.Add(obj.Key);
            if (items.Count > 10) break;
        }

        return items;
    }
}