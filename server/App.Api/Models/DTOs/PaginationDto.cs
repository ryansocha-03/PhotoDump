namespace App.Api.Models.DTOs;

/// <summary>
/// Generic DTO represented a cursor-paginated set of data.
/// </summary>
public sealed record PaginationDto<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public bool HasNext { get; set; }
    public string? NextCursor { get; set; }
}