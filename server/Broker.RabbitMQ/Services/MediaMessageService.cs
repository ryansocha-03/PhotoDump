using Broker.Abstractions.Interfaces;
using Broker.Abstractions.Models.Messages;
using Broker.RabbitMQ.Models.Configuration;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Broker.RabbitMQ.Services;

/// <summary>
/// Service implementation implementing function related to media messages.
/// </summary>
public class MediaMessageService(IOptions<RabbitMqClientConfiguration> options, ISendEndpointProvider sendEndpointProvider) : IMediaMessageService
{
    private readonly RabbitMqClientConfiguration _config = options.Value;
    
    /// <inheritdoc /> 
    public async Task PublishMediaUploadMessageAsync(string mediaPublicName, long mediaInternalId)
    {
        var targetQueue = new Uri($"queue:{_config.QueueName}");
        var endpoint = await sendEndpointProvider.GetSendEndpoint(targetQueue);

        await endpoint.Send(new NewMediaUploadMessage
        {
            MediaId = mediaInternalId,
            ObjectName = mediaPublicName
        });
    }
}