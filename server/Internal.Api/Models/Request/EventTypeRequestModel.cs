namespace Internal.Api.Models.Request;

/// <summary>
/// Request model containing fields for creating a new event type.
/// </summary>
public record EventTypeRequestModel
{
    /// <summary>
    /// The name of the new event type.
    /// </summary>
    public required string EventTypeName { get; init; }
}