using App.Api.Models.Request;
using Infrastructure.EntityFramework.Models;
using Infrastructure.EntityFramework.Repositories.Interfaces;

namespace App.Api.Services;

public class MediaService(IMediaRepository mediaRepository)
{
    public async Task<List<string>> UploadMedia(List<MediaUploadInfo> mediaUploadInfo, int eventId, bool isPrivate)
    {
        List<string> publicFileNames = new List<string>();
        IEnumerable<Media> mediaEntities = mediaUploadInfo.Select(m =>
            {
                var publicFileName = GeneratePublicFileName();
                publicFileNames.Add(publicFileName);
                return new Media
                {
                    FileName = m.FileName,
                    PublicFileName = publicFileName,
                    OriginalSize = m.FileSize,
                    IsPrivate = isPrivate,
                    EventId = eventId,
                };
            }
        );
        
        await mediaRepository.AddMultipleAsync(mediaEntities);

        return publicFileNames;
    }

    private string GeneratePublicFileName()
    {
        return Guid.NewGuid().ToString("N");
    }

    public async Task<IEnumerable<Media>> GetMediaForEvent(int eventId)
    {
        return await mediaRepository.GetAllAsync(eventId);
    }
}