using System.ComponentModel.DataAnnotations;

namespace Broker.RabbitMQ.Models.Configuration;

/// <summary>
/// Defines the shape of the configuration of a message broker client.
/// </summary>
public record RabbitMqClientConfiguration 
{
    [Required(AllowEmptyStrings = false)]
    public required string Host { get; init; }

    [Required]
    public required int Port { get; init; }

    [Required(AllowEmptyStrings = false)]
    public required string UserName { get; init; }

    [Required(AllowEmptyStrings = false)]
    public required string Password { get; init; }

    [Required(AllowEmptyStrings = false)]
    public required string QueueName { get; init; }
}