namespace Infrastructure.EntityFramework.Models.DTOs;

public class MediaStateTransitionDto
{
    public required int MediaInternalId { get; set; }
    public required bool IsPrivate { get; set; }
}