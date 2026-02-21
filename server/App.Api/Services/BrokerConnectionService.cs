using Core.Configuration.ConfigurationModels;
using Core.Interfaces;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace App.Api.Services;

public class BrokerConnectionService(IOptions<BrokerClientConfigurationModel> configuration): IBrokerConnection, IAsyncDisposable 
{
    private readonly ConnectionFactory _connectionFactory = new()
    {
        Uri = new Uri(configuration.Value.Url),
        UserName = configuration.Value.UserName,
        Password = configuration.Value.Password,
    };
    private IConnection? _connection;
    private readonly SemaphoreSlim _semaphoreSlim = new (1, 1);

    public async Task<IConnection> GetConnectionAsync()
    {
        if (_connection is { IsOpen: true })
            return _connection;
        
        await _semaphoreSlim.WaitAsync();
        try
        {
            if (_connection is not { IsOpen: true })
            {
                _connection = await _connectionFactory.CreateConnectionAsync();
            }
        }
        finally
        {
            _semaphoreSlim.Release();
        }

        // Declare the queue once when we first create the singleton connection.
        await using var channel = await _connection.CreateChannelAsync();
        await channel.QueueDeclareAsync(configuration.Value.QueueName, true, false, false, null);
        
        return _connection;
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
        {
            await _connection.DisposeAsync();
        }
    }
}