using App.Api.Models;
using App.Api.Models.Request;
using App.Api.Services.DTOs;
using ContentStore.MinIO.Utilities;
using Core.DTOs;
using Core.Models;
using Infrastructure.EntityFramework.Models;
using Infrastructure.EntityFramework.Models.DTOs;
using Infrastructure.EntityFramework.Repositories.Interfaces;

namespace App.Api.Services;

public class MediaService(IMediaRepository mediaRepository, MediaCursorService cursorService)
{
    private const int MaxMediaPageSize = 9;

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

    public async Task<PaginationDto<MediaNameDto>> GetPublicMediaPageAsync(int eventId, Guid expectedEventPublicId, string? cursor, int? limit)
    {
        var normalizedLimit = limit < MaxMediaPageSize ? limit.Value :  MaxMediaPageSize;
        var mediaId = cursorService.DecodeCursor(cursor, expectedEventPublicId);

        var mediaData = await mediaRepository.GetMediaObjectsAsync(eventId, false, normalizedLimit, mediaId);

        var mediaPaginationData = new PaginationDto<MediaNameDto>();
        if (mediaData.Count > normalizedLimit)
        {
            mediaData.RemoveAt(mediaData.Count - 1);
            var nextCursor = cursorService.EncodeCursor(new MediaCursorPayload
            {
                EventPublicId = expectedEventPublicId, 
                MediaId = mediaData[^1].Id
            });
            mediaPaginationData.NextCursor = nextCursor;
            mediaPaginationData.HasNext = true;
        }

        mediaPaginationData.Items = mediaData;
        
        return mediaPaginationData;
    }

    public List<Media> GetAllMediaForEvent(int eventId)
    {
        return mediaRepository.GetAll(eventId).ToList();
    }

    public async Task<List<MediaStateTransitionDto>> AcknowledgeUploadStateTransition(string publicFileName, Guid eventPublicId)
    {
        return await mediaRepository.MediaStateTransitionAsync(publicFileName, eventPublicId, UploadStatus.Pending, UploadStatus.Uploaded);
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