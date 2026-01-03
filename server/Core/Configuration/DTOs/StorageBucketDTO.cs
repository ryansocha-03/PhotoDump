namespace Core.Configuration.DTOs;

public class StorageBucketDto
{
    public required string BucketName { get; set; }
    public required DateTime BucketCreationDate { get; set; }
}