namespace App.Api.Models.Response;

public class SessionResponseModel
{
    public required string SessionId { get; set; }
    public required DateTimeOffset ExpiresAt { get; set; }
}