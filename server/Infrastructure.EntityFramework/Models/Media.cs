using Core.Models;

namespace Infrastructure.EntityFramework.Models;

public class Media
{
    public int Id { get; set; }
    
    public string FileName { get; set; }
    public string PublicFileName { get; set; } 
    public long OriginalSize { get; set; }
    public string Status { get; set; }
    public int UploadAttempts { get; set; }
    public int DownloadCount { get; set; }
    public bool IsPrivate { get; set; }
    public int EventId{ get; set; }
    public Event Event { get; set; }
    public int? GuestId { get; set; }
    public Guest? Guest { get; set; }
    public int MediaTypeId { get; set; }
    public MediaType MediaType { get; set; }
}