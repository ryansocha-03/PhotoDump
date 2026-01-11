namespace Core.Configuration.ConfigurationModels;

public class ContentStoreConfigurationModel
{
    public required string ContentStoreProvider { get; set; }
    public string AccessKey { get; set; } = "";
    public string SecretKey { get; set; } = "";
    public string Endpoint { get; set; } = "";
    public string Region { get; set; } = "";
}