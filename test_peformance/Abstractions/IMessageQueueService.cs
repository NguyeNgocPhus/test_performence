using test_peformance.Events;

namespace test_peformance.Abstractions;

public interface IMessageQueueService
{
    Task EnqueueAsync(QueueMessage message, CancellationToken cancellationToken = default);
    Task<QueueMessage> DequeueAsync(CancellationToken cancellationToken);
}