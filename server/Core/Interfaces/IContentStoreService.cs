using Core.Configuration.DTOs;

namespace Core.Interfaces;

public interface IContentStoreService
{
    public Task<List<StorageBucketDto>> ListBuckets();
}