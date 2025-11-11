namespace Admin.Api.Models;

public class NewEventRequestModel
{
    public required string EventName { get; set; }
    public required string EventNameShort { get; set; }
    public required string EventColorPrimary { get; set; }
    public required string EventColorSecondary { get; set; }
    public DateTime EventDate { get; set; }
    public int DurationDays { get; set; } = 1;
    public required string EventPassword { get; set; }
    public int EventStateId { get; set; } = 1;
    public int EventTypeId { get; set; } = 1;
}