namespace Infrastructure.EntityFramework.Models;

public class Event
{
    public int Id { get; set; }
    public Guid PublicId { get; set; }
    public string EventName { get; set; }
    public string? EventNameShort { get; set; }
    public string? ColorPrimary { get; set; }
    public string? ColorSecondary { get; set; }
    public DateTime EventDate { get; set; }
    public int DurationDays { get; set; }
    public string EventPasswordHash { get; set; }
    
    public int EventStateId { get; set; }
    public EventState EventState { get; set; }
    public int EventTypeId { get; set; }
    public EventType EventType { get; set; }
    public IEnumerable<Guest> Guests { get; set; } = new List<Guest>();
    public IEnumerable<Admin> Admins { get; set; } = new List<Admin>();
    
}