using Core.Configuration.DTOs;
using Core.Models;

namespace Core.Interfaces;

public interface IContentStoreService
{
    public Task<List<StorageBucketDto>> ListBuckets();
    
    public Task<string?> GeneratePresignedDownloadUrl(Guid eventId, FilePrivacyEnum privacy, string fileName);
    
    public Task<string?> GeneratePresignedUploadUrl(Guid eventId, FilePrivacyEnum privacy, string fileName);

    public Task<IEnumerable<string>> GenerateBulkPresignedUploadUrls(IEnumerable<string> fileNames, Guid eventId, FilePrivacyEnum privacy);
    
    public Task<IEnumerable<string>> GenerateBulkPresignedDownloadUrls(IEnumerable<MediaFileNameInfo> fileNames, Guid eventId, FilePrivacyEnum privacy);

    public Task<bool> DeleteMediaFromEvent(Guid eventId, FilePrivacyEnum privacy, string fileName);
    public Task<List<string>> ListObjectsInBucket(Guid eventId);

    public string BuildObjectName(Guid eventId, FilePrivacyEnum privacy, string fileName);
}