using photodump_api.Infrastructure.Db.Repositories;
using photodump_api.Shared.Interfaces;

namespace photodump_api.Features.UploadMedia;

public class UploadMediaHandler(IMediaStorage storageService, IMediaRepository mediaRepository, IGuestRepository guestRepository) : IFeatureHandler<UploadMediaModel, Uri>
{
    public async Task<Uri> HandleAsync(UploadMediaModel request)
    {
        var guest = await guestRepository.GetAsync(request.eventId, request.guestId);
        if (guest is null)
        {
            throw new BadHttpRequestException("Invalid event/guest");
        }
        
        bool metadataUploaded = await mediaRepository.BulkAddAsync(request.Files);
        if (!metadataUploaded)
        {
            throw new IOException("Unable to ingest file metadata");
        }
        
        return await storageService.RequestUpload();
    }
}