namespace App.Api.Models.Response;

public class EventLandingResponseModel
{
    public int Id { get; set; }
    public Guid EventPublicId { get; set; }
    public required string EventName { get; set; }
    public string EventNameShort { get; set; } = string.Empty;
    public string ColorPrimary { get; set; } = string.Empty;
    public string ColorSecondary { get; set; }  = string.Empty;
}