using System.Text;
using System.Text.Json;
using Core.Configuration.ConfigurationModels;
using Core.Interfaces;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace App.Api.Services;

public class BrokerService(IBrokerConnection connection, IOptions<BrokerClientConfigurationModel> configuration): IBrokerPublisher
{
    public async Task PublishAsync<T>(string routingKey, T message)
    {
        var conn = await connection.GetConnectionAsync();
        await using var channel = await conn.CreateChannelAsync();

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey:  routingKey,
            body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message))
        );
    }
}