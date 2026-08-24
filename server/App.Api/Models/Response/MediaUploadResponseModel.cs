namespace App.Api.Models.Response;

/// <summary>
/// Response model outlining data returned when uploading new media.
/// </summary>
public record MediaUploadResponseModel
{
    public required string PublicFileId { get; set; }
    public required string FileUploadUrl { get; set; }
}