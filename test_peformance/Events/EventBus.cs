using System.Threading.Channels;
using test_peformance.Abstractions;

namespace test_peformance.Events;

public sealed class EventBus : IEventBus
{
    private readonly InMemoryMessageQueue _inMemoryMessageQueue;
    private readonly Channel<IIntegrationEvent> _channel = Channel.CreateUnbounded<IIntegrationEvent>();
    public EventBus(InMemoryMessageQueue inMemoryMessageQueue)
    {
        _inMemoryMessageQueue = inMemoryMessageQueue;
    }

    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken) where T : class, IIntegrationEvent
    {
        _channel.Writer.TryWrite(@event);
        await _inMemoryMessageQueue.Writer.WriteAsync(@event, cancellationToken);
        
    }

    public async Task GetAllEventAsync(CancellationToken cancellationToken)
    {
        await foreach (var item in _channel.Reader.ReadAllAsync(cancellationToken: cancellationToken))
        {
            Console.WriteLine($"data la : {item.EventId}");
        }
        // _channel.Reader.TryRead()
       // Console.WriteLine($"{nameof(EventBus)}.{nameof(GetAllEventAsync)} {a1.EventId}");
       
    }
}