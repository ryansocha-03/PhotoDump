namespace App.Api.Models.Response;

public class MediaUploadResponseModel
{
    public required string PublicFileId { get; set; }
    public required string FileUploadUrl { get; set; }
}