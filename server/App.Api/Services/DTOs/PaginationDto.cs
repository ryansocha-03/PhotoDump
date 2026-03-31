namespace App.Api.Services.DTOs;

public class PaginationDto<T>
{
    public List<T> Items { get; set; } = [];
    public bool HasNext { get; set; } = false;
    public string? NextCursor { get; set; }
}