using Domain.Enums;

namespace Internal.Api.Models.Request;

/// <summary>
/// Request model for creating a new event.
/// </summary>
public class UpdateEventRequestModel
{
    public string? EventName { get; init; }
    public string? EventNameShort { get; init; }
    public string? ColorPrimary { get; init; }
    public string? ColorSecondary { get; init; }
    public DateTimeOffset? EventStartDate { get; init; }
    public DateTimeOffset? EventEndDate { get; init; }
    public string? EventPassword { get; init; }
    public EventStateEnum EventState { get; init; }
    public long EventTypeId { get; init; } 
}