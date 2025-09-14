namespace photodump_api.Domain;

public class Media
{
    public int Id { get; set; }
    public required string Path { get; set; }
    public int EventId { get; set; }
    public Event Event { get; set; }
    public int GuestId { get; set; }
    public Guest Guest { get; set; }
    public bool IsPrivate { get; set; }
    public DateTime UploadDate { get; private set; }
}