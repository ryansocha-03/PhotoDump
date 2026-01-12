namespace Core.Configuration.ConfigurationModels;

public class ContentStoreConfigurationModel
{
    public required string ContentStoreProvider { get; set; }
    public string AccessKey { get; set; } = "";
    public string SecretKey { get; set; } = "";
    public string Endpoint { get; set; } = "";
    public string Region { get; set; } = "";
    public string Bucket { get; set; } = "photodump";
    public int PresignedDownloadDurationMinutes { get; set; } = 10;
    public int PresignedUploadDurationMinutes { get; set; } = 10;
}