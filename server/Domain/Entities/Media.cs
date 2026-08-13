using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Entity model defining a piece of Media.
/// </summary>
public class Media
{
    public long Id { get; set; }
    
    public required string FileName { get; set; }
    public required string PublicFileName { get; set; } 
    public long OriginalSize { get; set; }
    public ContentStatusEnum Status { get; set; }
    public int UploadAttempts { get; set; }
    public int DownloadCount { get; set; }
    public FilePrivacyEnum IsPrivate { get; set; }
    public ContentTypeEnum ContentType { get; set; }
    
    #region Foreign Keys
    
    public long EventId { get; set; }
    
    #endregion
    
    public Event? Event { get; private set; }
}