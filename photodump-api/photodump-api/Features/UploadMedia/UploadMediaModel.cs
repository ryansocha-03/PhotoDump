namespace photodump_api.Features.UploadMedia;

public class UploadMediaModel
{
    public required List<FileUploadData> Files { get; set; }
    public required int eventId { get; set; }
    public required int guestId { get; set; }
}