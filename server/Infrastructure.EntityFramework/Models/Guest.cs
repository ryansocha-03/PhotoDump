namespace Infrastructure.EntityFramework.Models;

public class Guest
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }
    
    public int EventId { get; set; }
    public Event Event { get; set; }
    
    public IEnumerable<Media> UploadedMedia { get; set; } = new  List<Media>();
}