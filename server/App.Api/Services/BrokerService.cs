using Core.Configuration.ConfigurationModels;
using Core.Interfaces;
using Microsoft.Extensions.Options;

namespace App.Api.Services;

public class BrokerService(IOptions<BrokerClientConfigurationModel> configuration): IBrokerPublisher
{
    private BrokerClientConfigurationModel _configuration = configuration.Value;
}