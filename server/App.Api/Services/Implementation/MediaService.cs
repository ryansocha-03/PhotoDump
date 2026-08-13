using System.Text.Json;
using App.Api.Models.DTOs;
using App.Api.Services.Definition;
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

    #endregion
}