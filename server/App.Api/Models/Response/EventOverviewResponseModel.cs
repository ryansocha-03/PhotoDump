namespace App.Api.Models.Response;

/// <summary>
/// Response model for providing overview information about an Event.
/// </summary>
public record EventOverviewResponseModel
{
    public required Guid EventPublicId { get; init; }
    public required string EventName { get; init; }
    public string? EventNameShort { get; init; }
    public string? ColorPrimary { get; init; } 
    public string? ColorSecondary { get; init; } 
}