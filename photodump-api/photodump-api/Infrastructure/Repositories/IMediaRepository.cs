using photodump_api.Features.UploadMedia;

namespace photodump_api.Infrastructure.Db.Repositories;

public interface IMediaRepository
{
    public Task<bool> BulkAddAsync(List<FileUploadData> fileData);
}