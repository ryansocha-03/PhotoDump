namespace Internal.Api.Models.Request;

/// <summary>
/// Request model for creating a new event.
/// </summary>
public record NewEventRequestModel
{
    public required string EventName { get; init; }
    public string? EventNameShort { get; init; }
    public string? ColorPrimary { get; init; }
    public string? ColorSecondary { get; init; }
    public required DateTimeOffset EventStartDate { get; init; }
    public required DateTimeOffset EventEndDate { get; init; }
    public required string EventPassword { get; init; }
    public long EventTypeId { get; init; }
}