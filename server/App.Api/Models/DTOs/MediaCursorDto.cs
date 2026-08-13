namespace App.Api.Models.DTOs;

/// <summary>
/// DTO representing the required fields for building a Media cursor
/// </summary>
public sealed record MediaCursorDto
{
    public required Guid EventPublicId { get; init; }
    public required long Id { get; init; }
}