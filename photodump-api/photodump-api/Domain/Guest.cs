namespace photodump_api.Domain;

public class Guest
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    
    public Event Event { get; set; }
    public int EventId { get; set; }

    public List<Media> MediaUploads { get; set; } = new();
}