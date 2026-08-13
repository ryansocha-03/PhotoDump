namespace App.Api.Models.Request;

/// <summary>
/// Request model outlining fields for logging into an event.
/// </summary>
public record EventLoginRequestModel
{
    public required string EventKey { get; init; } 
}