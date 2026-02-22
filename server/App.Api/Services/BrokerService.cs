using System.Text;
using System.Text.Json;
using Core.Configuration.ConfigurationModels;
using Core.Interfaces;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace App.Api.Services;

public class BrokerService(IBrokerConnection connection): IBrokerPublisher
{
    public async Task PublishAsync<T>(string routingKey, T message)
    {
        var conn = await connection.GetConnectionAsync();
        await using var channel = await conn.CreateChannelAsync();
        channel.BasicReturnAsync += (sender, ea) =>
        {
            Console.Error.WriteLine("Message was returned"); // TODO: Handle unroutable messages better
            return null!;
        };

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey:  routingKey,
            mandatory: true,
            basicProperties: new BasicProperties{ Persistent = true },
            body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message))
        );
    }
}