namespace App.Api.Infrastructure.Models;

public class Media
{
    public int Id { get; set; }
    
    public string FileName { get; set; }
    public long OriginalSize { get; set; }
    public bool IsDeleted { get; set; }
    public int DownloadCount { get; set; }
    public string Url { get; set; }
    public bool IsPrivate { get; set; }
    
    public int GuestId { get; set; }
    public Guest Guest { get; set; }
    public int MediaTypeId { get; set; }
    public MediaType MediaType { get; set; }
}