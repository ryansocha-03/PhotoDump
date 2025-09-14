namespace photodump_api.Features.UploadMedia;

public class UploadMediaRequestDto
{
    public required List<FileUploadData> Files { get; set; }
}

public class FileUploadData
{
    public required string FileName { get; set; }
    public required string FileExtension { get; set; }
    public required long FileSize { get; set; }
    public bool IsPrivate { get; set; } = false;
}

public class UploadMediaResponseDto
{
    public required string SignedUrl { get; set; }
    
}