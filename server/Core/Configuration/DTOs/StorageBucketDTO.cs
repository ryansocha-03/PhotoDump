namespace Core.Configuration.DTOs;

public class StorageBucketDTO
{
    public required string BucketName { get; set; }
    public required DateTime BucketCreationDate { get; set; }
}