namespace App.Api.Models.Response;

/// <summary>
/// Response model for providing information about an existing or newly created session.
/// </summary>
public class SessionResponseModel
{
    public required string SessionId { get; init; }
    public required DateTimeOffset ExpiresAt { get; init; }
}