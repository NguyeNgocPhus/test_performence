using RabbitMQ.Client;

namespace test_peformance.Infrastructure.Messaging.RabbitMQ;

public interface IRabbitMqPersistentConnection : IDisposable
{
    bool IsConnected { get; }
    bool TryConnect();
    IModel CreateModel();
}
