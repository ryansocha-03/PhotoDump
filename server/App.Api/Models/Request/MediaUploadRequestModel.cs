namespace App.Api.Models.Request;

public class MediaUploadRequestModel
{
    public required List<MediaUploadInfo> MediaUploadInfo { get; set; }
    public bool IsPrivate { get; set; } = true;
}

public class MediaUploadInfo 
{
    public required string FileName { get; set; }
    public required long FileSize { get; set; }
    public required string MimeType { get; set; }
}