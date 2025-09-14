namespace photodump_api.Domain;

public class Event
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required DateTime EventDate { get; set; }

    public List<Guest> GuestList { get; set; } = new();
    public List<Media> MediaList { get; set; } = new();
}