namespace Core.Interfaces;

public interface IBrokerPublisher
{
    Task PublishAsync<T>(string routingKey, T message);
}