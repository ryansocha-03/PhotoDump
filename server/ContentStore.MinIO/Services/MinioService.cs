using System.Collections.Immutable;
using ContentStore.Abstractions.Models;
using ContentStore.MinIO.Models.Configuration;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using ContentStore.Abstractions.Interfaces;
using ContentStore.Abstractions.Models.Configuration;
using Domain.Enums;
using Domain.Extensions;
using Minio.Exceptions;

namespace ContentStore.MinIO.Services;

/// <summary>
/// Service implementation of a <see cref="IContentStoreService"/> using MinIO
/// </summary>
public class MinioService(IOptions<MinIoConfiguration> minIoOptions, IOptions<ContentStoreConfiguration> contentStoreOptions) : IContentStoreService 
{
    private readonly MinIoConfiguration _minIoConfiguration = minIoOptions.Value;
    private readonly ContentStoreConfiguration _contentStoreConfiguration = contentStoreOptions.Value;
    
    private readonly IMinioClient _externalS3Client = new MinioClient()
        .WithEndpoint(minIoOptions.Value.ExternalEndpoint)
        .WithRegion(minIoOptions.Value.Region)
        .WithCredentials(minIoOptions.Value.AccessKey, minIoOptions.Value.SecretKey)
        .WithSSL(minIoOptions.Value.UseSsl)
        .Build();
    
    private readonly IMinioClient _internalS3Client = new MinioClient()
        .WithEndpoint(minIoOptions.Value.InternalEndpoint)
        .WithRegion(minIoOptions.Value.Region)
        .WithCredentials(minIoOptions.Value.AccessKey, minIoOptions.Value.SecretKey)
        .WithSSL(minIoOptions.Value.UseSsl)
        .Build();

    /// <inheritdoc/> 
    public async Task<IReadOnlyCollection<StorageBucket>> ListBucketsAsync()
    {
        return (await _internalS3Client.ListBucketsAsync())
            .Buckets
            .Select(bucket => new StorageBucket(bucket.Name, bucket.CreationDateDateTime))
            .ToImmutableList();
    }

    /// <inheritdoc /> 
    public async Task<IReadOnlyCollection<string>> CreateUploadsAsync(ContentKeyGroup objects)
    {
        var args = new PresignedPutObjectArgs()
            .WithBucket(_minIoConfiguration.Bucket)
            .WithExpiry(_contentStoreConfiguration.UploadWindowMinutes * 60);

        var urls = new List<string>();
        foreach (var file in objects.ObjectNames)
        {
            args.WithObject(GetObjectLocation(new ContentKey(objects.OwnerId, objects.Privacy, objects.ContentVariant, file)));
            urls.Add(await _externalS3Client.PresignedPutObjectAsync(args));
        }

        return urls.ToImmutableList();
    }

    /// <inheritdoc /> 
    public async Task<IReadOnlyCollection<string>> CreateDownloadsAsync(ContentKeyGroup objects)
    {
        var args = new PresignedGetObjectArgs()
            .WithBucket(_minIoConfiguration.Bucket)
            .WithExpiry(_contentStoreConfiguration.DownloadWindowMinutes * 60);
        
        var urls = new List<string>();
        foreach (var file in objects.ObjectNames)
        {
            args.WithObject(GetObjectLocation(new ContentKey(objects.OwnerId, objects.Privacy, objects.ContentVariant, file)))
                .WithHeaders(new Dictionary<string, string>
                {
                    { "Content-Disposition", $"attachment; filename=\"{file}\"" }
                });
            urls.Add(await _internalS3Client.PresignedGetObjectAsync(args));
        }
        
        return urls.ToImmutableList();
    }

    /// <inheritdoc />
    public async Task<bool> DeleteContentAsync(ContentKeyGroup objects)
    {
        var args = new RemoveObjectArgs()
            .WithBucket(_minIoConfiguration.Bucket);

        foreach (var content in objects.ObjectNames)
        {
            args.WithObject(GetObjectLocation(new ContentKey(objects.OwnerId, objects.Privacy, objects.ContentVariant, content)));
            try
            {
                await _internalS3Client.RemoveObjectAsync(args);
            }
            catch (InvalidObjectNameException)
            {
                return false;
            }
        }

        return true;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteContentExactAsync(string objectName)
    {
        var args = new RemoveObjectArgs()
            .WithBucket(_minIoConfiguration.Bucket)
            .WithObject(objectName);

        try
        {
            await _internalS3Client.RemoveObjectAsync(args);
        }
        catch (InvalidObjectNameException)
        {
            return false;
        }
        return true;
    }

    /// <inheritdoc />
    public string GetObjectLocation(ContentKey objectKey)
    {
        var suffix = objectKey.ContentVariant switch
        {
            ContentVariantEnum.Gallery => "/gallery",
            ContentVariantEnum.Spotlight => "/spotlight",
            ContentVariantEnum.Original => " /original",
            _ => throw new ArgumentOutOfRangeException(nameof(objectKey.ContentVariant), objectKey.ContentVariant, null)
        };

        return $"{objectKey.OwnerId}/{objectKey.Privacy.ToMemberValue().ToLower()}/{objectKey.ObjectName}{suffix}";
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<string>> GetContentNamesAsync(string location)
    {
        var args = new ListObjectsArgs()
            .WithBucket(_minIoConfiguration.Bucket)
            .WithRecursive(true)
            .WithPrefix(location);

        var objects = _internalS3Client.ListObjectsEnumAsync(args);

        var contentNames = new List<string>();
        await foreach (var obj in objects)
        {
            contentNames.Add(obj.Key);
        }
        
        return contentNames.ToImmutableList();
    }
}