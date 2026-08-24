using System.Text.Json;
using App.Api.Models.DTOs;
using App.Api.Models.Request;
using App.Api.Services.Definition;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.DataProtection;
using Persistence.Abstractions.Interfaces.Repositories;

namespace App.Api.Services.Implementation;

/// <summary>
/// Service implementation containing functions relating to Media operations. 
/// </summary>
public class MediaService(IDataProtectionProvider dataProtectionProvider, IMediaRepository mediaRepository) : IMediaService
{
    private const string MediaDataProtectorPurpose = "media-download-protector";
    private const int MediaMaxPageSize = 15;
    
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector(MediaDataProtectorPurpose);
    
    /// <inheritdoc />
    public async Task<PaginationDto<string>> GetEventPublicMediaPagedAsync(long eventInternalId, Guid eventPublicId, string? encodedCursor, int? limit = null)
    {
        var normalizedLimit = limit == null ? MediaMaxPageSize : Math.Min(limit.Value, MediaMaxPageSize);
        long? normalizedCursor = string.IsNullOrWhiteSpace(encodedCursor)
            ? null
            : DecodeMediaCursor(encodedCursor, eventPublicId).Id;

        var mediaList = (await mediaRepository.GetMediaPagedAsync(eventInternalId, FilePrivacyEnum.Public,
            ContentStatusEnum.Completed, normalizedLimit, normalizedCursor)).ToList();

        var paginatedMedia = new PaginationDto<string>();
        if (mediaList.Count > normalizedLimit)
        {
            var lastMedia = mediaList.Last();
            paginatedMedia.NextCursor = EncodeMediaCursor(new MediaCursorDto
            {
                Id = lastMedia.Id,
                EventPublicId = eventPublicId
            });
            paginatedMedia.HasNext = true;
            mediaList.RemoveAt(mediaList.Count - 1);
        }

        paginatedMedia.Items = mediaList.Select(m => m.PublicFileName);

        return paginatedMedia;
    }

    /// <inheritdoc />
    public async Task<List<string>> CreateNewMediaEntriesAsync(long eventInternalId, FilePrivacyEnum privacy, List<MediaUploadInfo> mediaUploadInfos)
    {
        List<string> newPublicFileNames = [];
        List<Media> newMedia = [];
        
        foreach (var newMediaInfo in mediaUploadInfos)
        {
            var currentContentType = ValidateFileType(newMediaInfo);
            if (currentContentType == null)
            {
                throw new ArgumentException($"File type is invalid for {newMediaInfo.FileName}");
            }

            var newMediaPublicFileName = GenerateMediaPublicName();
            newPublicFileNames.Add(newMediaPublicFileName);
            newMedia.Add(new Media
            {
                FileName = newMediaInfo.FileName,
                PublicFileName = newMediaPublicFileName,
                OriginalSize = newMediaInfo.FileSize,
                IsPrivate = privacy,
                EventId = eventInternalId,
                Status = ContentStatusEnum.Pending,
                UploadAttempts = 0,
                DownloadCount = 0,
                ContentType = (ContentTypeEnum)currentContentType
            });
        }

        await mediaRepository.CreateBulkAsync(newMedia);

        return newPublicFileNames;
    }

    /// <inheritdoc />
    public async Task<MediaStateTransitionDto?> AcknowledgeMediaUploadAsync(string publicFileName, Guid eventPublicId)
    {
        var updatedMedia = await mediaRepository.UpdateMediaStateByNameAsync(publicFileName, eventPublicId,
            ContentStatusEnum.Pending, ContentStatusEnum.Uploaded);

        return updatedMedia == null ?
            null
            : new MediaStateTransitionDto
            {
                MediaInternalId = updatedMedia.Id,
                Privacy = updatedMedia.IsPrivate
            };
    }

    /// <inheritdoc />
    public async Task<MediaStateTransitionDto?> AcknowledgeMediaProcessingCompletionAsync(long mediaInternalId)
    {
        var updatedMedia = await mediaRepository.UpdateMediaStateByIdAsync(mediaInternalId, ContentStatusEnum.Uploaded,
            ContentStatusEnum.Completed);

        return updatedMedia == null
            ? null
            : new MediaStateTransitionDto
            {
                MediaInternalId = updatedMedia.Id,
                Privacy = updatedMedia.IsPrivate
            };
    }

    #region Private Helpers

    /// <summary>
    /// Decodes the provided media cursor string into its <see cref="MediaCursorDto"/>
    /// </summary>
    /// <param name="cursor">The encoded cursor as a string.</param>
    /// <param name="expectedEventPublicId">The public event identifier the cursor is expected to be scoped to.</param>
    /// <returns>The decoded <see cref="MediaCursorDto"/>.</returns>
    /// <exception cref="ArgumentException">If the decoding or deserialization fails, or if the cursors, event identifier doesn't match the expected.</exception>
    private MediaCursorDto DecodeMediaCursor(string cursor, Guid expectedEventPublicId)
    {
        var decodedCursor = _dataProtector.Unprotect(cursor);
        var mediaCursor = JsonSerializer.Deserialize<MediaCursorDto>(decodedCursor);

        if (mediaCursor == null || mediaCursor.EventPublicId != expectedEventPublicId)
        {
            throw new ArgumentException("Invalid media cursor.");
        }
        
        return mediaCursor;
    }

    /// <summary>
    /// Encodes the provided media cursor.
    /// </summary>
    /// <param name="mediaCursor">The <see cref="MediaCursorDto"/> to encode.</param>
    /// <returns>The encoded cursor as a <see cref="string"/>.</returns>
    private string EncodeMediaCursor(MediaCursorDto mediaCursor)
    {
        return _dataProtector.Protect(JsonSerializer.Serialize(mediaCursor));
    }

    /// <summary>
    /// Validates the file type of new <see cref="MediaUploadInfo"/>
    /// </summary>
    /// <param name="mediaUploadInfo">The <see cref="MediaUploadInfo"/> to validate.</param>
    /// <returns>A <see cref="ContentTypeEnum"/> if the <see cref="MediaUploadInfo"/> is a valid upload,
    /// <see langword="null"/> otherwise.</returns>
    private static ContentTypeEnum? ValidateFileType(MediaUploadInfo mediaUploadInfo)
    {
        if (Enum.TryParse(mediaUploadInfo.FileExtension, out ContentTypeEnum fileContentType))
        {
            return fileContentType;
        }
        
        return null;
    }
    
    /// <summary>
    /// Generates a new public media name.
    /// </summary>
    /// <returns>The new public file name as a <see langword="string"/></returns>
    private static string GenerateMediaPublicName()
    {
        return Guid.NewGuid().ToString("N");
    }
    
    #endregion
}