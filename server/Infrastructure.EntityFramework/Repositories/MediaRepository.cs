using Domain.Entities;
using Domain.Enums;
using Infrastructure.EntityFramework.Contexts;
using Microsoft.EntityFrameworkCore;
using Persistence.Abstractions.Interfaces.Repositories;

namespace Infrastructure.EntityFramework.Repositories;

/// <summary>
/// Repository implementation for interacting with <see cref="Media"/> entities using EF Core.
/// </summary>
public class MediaRepository(AppDbContext context) : IMediaRepository
{
    /// <inheritdoc /> 
    public async Task<IReadOnlyCollection<Media>> GetAllAsync()
    {
        return await context.Media.ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Media>> GetAllForEventAsync(long eventId)
    {
        return await context.Media.Where(m => m.EventId == eventId).ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Media>> GetAllForEventAsync(long eventId, FilePrivacyEnum privacy)
    {
        return await context.Media.Where(m => m.EventId == eventId && m.IsPrivate == privacy).ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Media?> GetByIdAsync(long id)
    {
        return await context.Media.FindAsync(id);
    }

    /// <inheritdoc />
    public async Task<Media?> GetByFileNameAndEventAsync(string fileName, long eventId)
    {
        return await context.Media.Where(
            m => m.PublicFileName == fileName 
                 && m.EventId == eventId)
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Media>> GetMediaPagedAsync(long eventId, FilePrivacyEnum privacy, ContentStatusEnum status, int limit, long? cursor)
    {
        return await context.Media
            .Where(m => m.EventId == eventId && m.IsPrivate == privacy && m.Status == status && (!cursor.HasValue || m.Id < cursor.Value))
            .OrderByDescending(m => m.Id)
            .Take(limit + 1)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Media> CreateAsync(Media entity)
    {
        var resolvedMedia = await context.Media.AddAsync(entity);
        await context.SaveChangesAsync();
        return resolvedMedia.Entity;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Media>> CreateBulkAsync(IEnumerable<Media> entities)
    {
        var mediaList = entities.ToList();
        context.Media.AddRange(mediaList);
        await context.SaveChangesAsync();
        return mediaList;
    }

    /// <inheritdoc />
    public async Task<Media?> UpdateAsync(Media updatedMedia)
    {
        var rowsUpdated = await context.Media
            .Where(m => m.Id == updatedMedia.Id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(m => m.FileName, updatedMedia.FileName)
                .SetProperty(m => m.PublicFileName, updatedMedia.PublicFileName)
                .SetProperty(m => m.OriginalSize, updatedMedia.OriginalSize)
                .SetProperty(m => m.Status, updatedMedia.Status)
                .SetProperty(m => m.UploadAttempts, updatedMedia.UploadAttempts)
                .SetProperty(m => m.DownloadCount, updatedMedia.DownloadCount)
                .SetProperty(m => m.IsPrivate, updatedMedia.IsPrivate)
                .SetProperty(m => m.ContentType, updatedMedia.ContentType));

        return rowsUpdated == 0 ? null : updatedMedia;
    }

    /// <inheritdoc />
    public async Task<Media?> UpdateMediaStateByIdAsync(long mediaId, ContentStatusEnum currentStatus, ContentStatusEnum desiredStatus)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var updatedMedia= await context.Media.FromSqlRaw(@"
                UPDATE ""Media"" 
                SET ""Status"" = {0}
                WHERE m.""Id"" = {1}
                    AND m.""Status"" = {2}
                RETURNING *", desiredStatus, mediaId, currentStatus)
                .AsNoTracking()
                .ToListAsync();

            if (updatedMedia.Count != 1)
            {
                throw new Exception();
            }
            
            await transaction.CommitAsync();
            return updatedMedia[0];
        }
        catch
        {
            await transaction.RollbackAsync();
            throw new InvalidOperationException("Unexpected number of update operations");
        }
    }

    /// <inheritdoc />
    public async Task<Media?> UpdateMediaStateByNameAsync(string fileName, Guid publicEventId, ContentStatusEnum currentStatus,
        ContentStatusEnum desiredStatus)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var updatedMedia = await context.Media.FromSqlRaw(@"
                UPDATE ""Media"" m
                SET ""Status"" = {0}
                FROM ""Events"" e
                WHERE m.""EventId"" = e.""Id""
                    AND m.""PublicFileName"" = {1}
                    AND m.""Status"" = {2}
                    AND e.""PublicId"" = {3}
                RETURNING m.*", desiredStatus, fileName, currentStatus, publicEventId)
                .AsNoTracking()
                .ToListAsync();

            if (updatedMedia.Count != 1)
            {
                throw new Exception();
            }
            
            await transaction.CommitAsync();
            return updatedMedia[0];
        }
        catch
        {
            await transaction.RollbackAsync();
            throw new InvalidOperationException("Unexpected number of update operations");
        }
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(long id)
    {
        var rowsUpdated = await context.Media
            .Where(m => m.Id == id)
            .ExecuteDeleteAsync();
        
        return rowsUpdated != 0;
    }
}