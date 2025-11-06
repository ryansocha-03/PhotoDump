namespace Infrastructure.EntityFramework.Models;

public class EventState
{
    public int Id { get; set; }
    public string StateName { get; set; }

    public IEnumerable<Event> Events { get; set; } = new List<Event>();
}