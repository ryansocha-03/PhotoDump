using App.Api.Models.Request;
using ContentStore.MinIO.Utilities;
using Core.Models;
using Infrastructure.EntityFramework.Models;
using Infrastructure.EntityFramework.Repositories.Interfaces;

namespace App.Api.Services;

public class MediaService(IMediaRepository mediaRepository)
{
    public async Task<List<string>> UploadMedia(List<MediaUploadInfo> mediaUploadInfo, int eventId, bool isPrivate)
    {
        List<string> publicFileNames = [];
        List<Media> mediaEntities = [];
        
        foreach (var media in mediaUploadInfo)
        {
            // TODO: Better supported file type check
            var fileExtension = GetFileExtension(media.FileName);
            if (!IsValidFileType(fileExtension))
                return [];

            var publicFileName = GeneratePublicFileName();
            publicFileNames.Add(publicFileName);
            mediaEntities.Add(new Media
            {
                FileName = media.FileName,
                PublicFileName = publicFileName,
                OriginalSize = media.FileSize,
                IsPrivate = isPrivate,
                EventId = eventId,
                MediaTypeId = 1
            });
        }

        await mediaRepository.AddMultipleAsync(mediaEntities);

        return publicFileNames;
    }

    public List<Media> GetAllMediaForEvent(int eventId)
    {
        return mediaRepository.GetAll(eventId).ToList();
    }

    public List<MediaFileNameInfo> GetMediaForEvent(int eventId,  bool isPrivate)
    {
        var mediaData = mediaRepository.GetAll(eventId, isPrivate);
        return mediaData.Select(media => new MediaFileNameInfo
            { DownloadFileName = media.FileName, UrlFileName = media.PublicFileName }).ToList();
    }

    public async Task<int> AcknowledgeUploadStateTransition(string publicFileName, int eventId)
    {
        return await mediaRepository.MediaUploadStateTransition(publicFileName, eventId, UploadStatus.Pending, UploadStatus.Uploaded);
    } 
    public async Task<bool> DeleteMedia(int id)
    {
        return await mediaRepository.DeleteAsync(id);
    }

    public async Task<Media?> GetSpecificMedia(int mediaId)
    {
        return await mediaRepository.GetAsync(mediaId);
    }

    private static string GetFileExtension(string fileName)
    {
        return Path.GetExtension(fileName);
    }
    
    private static bool IsValidFileType(string fileExtension)
    {
        return SupportedFileTypes.SupportedFileExtensions.Contains(fileExtension);
    } 

    private static string GeneratePublicFileName()
    {
        return Guid.NewGuid().ToString("N");
    }

    
}