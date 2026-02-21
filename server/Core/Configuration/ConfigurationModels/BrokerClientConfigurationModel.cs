namespace Core.Configuration.ConfigurationModels;

public class BrokerClientConfigurationModel
{
    public required string Url { get; set; }
    public required string UserName { get; set; }
    public required string Password { get; set; }
    public required string QueueName { get; set; }
}