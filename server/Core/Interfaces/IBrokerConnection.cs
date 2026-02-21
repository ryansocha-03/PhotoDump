using RabbitMQ.Client;

namespace Core.Interfaces;

public interface IBrokerConnection
{
    Task<IConnection> GetConnectionAsync();
}