namespace Core.Models;

public class ProcessMediaThumbnailMessageModel
{
    public required string ObjectName { get; set; } // event public id, privacy, and public file name
    public required int MediaId { get; set; }
}