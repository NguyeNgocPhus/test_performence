using test_peformance.Abstractions;
using test_peformance.Events;

namespace test_peformance;

public class IntegrationEventProcessJob : BackgroundService
{
    private readonly IEventBus _eventBus;
    private readonly InMemoryMessageQueue _inMemoryMessageQueue;

    public IntegrationEventProcessJob(IEventBus eventBus, InMemoryMessageQueue inMemoryMessageQueue)
    {
        _eventBus = eventBus;
        _inMemoryMessageQueue = inMemoryMessageQueue;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var item in _inMemoryMessageQueue.Reader.ReadAllAsync(cancellationToken: stoppingToken))
        {
            Console.WriteLine($"data la : {item.EventId}");
        }
    }
}