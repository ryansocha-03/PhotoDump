namespace App.Api.Models;

public sealed class MediaCursorPayload
{
    public required Guid EventPublicId { get; init; }
    public required int MediaId { get; init; } 
}