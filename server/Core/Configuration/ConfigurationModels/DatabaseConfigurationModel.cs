namespace Core.Configuration.ConfigurationModels;

public class DatabaseConfigurationModel
{
    public string DatabaseProvider { get; set; } = string.Empty;
    public string ConnectionString { get; set; } =  string.Empty;
}