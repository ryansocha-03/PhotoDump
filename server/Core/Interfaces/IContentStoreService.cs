using Core.Configuration.DTOs;
using Core.Models;

namespace Core.Interfaces;

public interface IContentStoreService
{
    public Task<List<StorageBucketDto>> ListBuckets();
    
    public Task<string?> GeneratePresignedDownloadUrl(Guid eventId, FilePrivacyEnum privacy, string fileName);
    
    public Task<string?> GeneratePresignedUploadUrl(Guid eventId, FilePrivacyEnum privacy, string fileName);
    
    public Task<List<string>> ListObjectsInBucket(Guid eventId);
}