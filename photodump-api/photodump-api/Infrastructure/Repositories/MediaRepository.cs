using photodump_api.Features.UploadMedia;

namespace photodump_api.Infrastructure.Db.Repositories;

public class MediaRepository: IMediaRepository
{
    public Task<bool> BulkAddAsync(List<FileUploadData> fileData)
    {
        return Task.FromResult(true);
    }
}