using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Entity model defining an Event.
/// </summary>
public class Event
{
    public long Id { get; set; }
    public Guid PublicId { get; set; }
    public required string EventName { get; set; }
    public string? EventNameShort { get; set; }
    public string? ColorPrimary { get; set; }
    public string? ColorSecondary { get; set; }
    public required DateTimeOffset StartDate { get; set; }
    public required DateTimeOffset EndDate { get; set; }
    public required string EventPasswordHash { get; set; }
    public required EventStateEnum  EventState { get; set; }
    
    #region Foreign Keys
    
    public long EventTypeId { get; set; }
    
    #endregion

    public EventType? EventType { get; private set; }
    public IEnumerable<Admin>? Admins { get; private set; }
    public IEnumerable<Media>? Media { get; private set; }
    public IEnumerable<EventSession>? EventSession { get; private set; }
}