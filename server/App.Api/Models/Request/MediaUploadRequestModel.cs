using Domain.Enums;

namespace App.Api.Models.Request;

/// <summary>
/// Request model outlining fields for uploading a set of media for an event.
/// </summary>
public sealed record MediaUploadRequestModel
{
    public required List<MediaUploadInfo> MediaUploadInfo { get; init; }
    public FilePrivacyEnum Privacy { get; init; } = FilePrivacyEnum.Private;
}

public sealed record MediaUploadInfo 
{
    public required string FileName { get; init; }
    public required string FileExtension { get; init; }
    public required long FileSize { get; init; }
}