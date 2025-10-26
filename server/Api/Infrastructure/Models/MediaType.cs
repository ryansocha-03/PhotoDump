namespace Api.Infrastructure.Models;

public class MediaType
{
    public int Id { get; set; }
    public string FileExtension { get; set; }
    
    public IEnumerable<Media> Media { get; set; } = new List<Media>();
}