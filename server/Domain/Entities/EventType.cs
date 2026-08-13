namespace Domain.Entities;

/// <summary>
/// Entity model defining an Event Type.
/// </summary>
public class EventType
{
    public long Id { get; set; }
    public required string TypeName { get; set; }

    public IEnumerable<Event>? Events { get; set; }
}