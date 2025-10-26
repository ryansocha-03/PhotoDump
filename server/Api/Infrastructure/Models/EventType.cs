namespace Api.Infrastructure.Models;

public class EventType
{
    public int Id { get; set; }
    public string TypeName { get; set; }

    public IEnumerable<Event> Events { get; set; } = new List<Event>();
}