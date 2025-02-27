using test_peformance.Abstractions;
using test_peformance.Events;

namespace test_peformance;

public class MessageQueueService : IMessageQueueService
{
    private readonly InMemoryMessageQueue _inMemoryMessageQueue;
    private readonly ILogger<MessageQueueService> _logger;

    public MessageQueueService(InMemoryMessageQueue inMemoryMessageQueue, ILogger<MessageQueueService> logger)
    {
        _inMemoryMessageQueue = inMemoryMessageQueue;
        _logger = logger;
    }

    public async Task EnqueueAsync(QueueMessage message, CancellationToken cancellationToken)
    {
        await _inMemoryMessageQueue.Writer.WriteAsync(message, cancellationToken);
        _logger.LogInformation("Message {MessageId} enqueued", message.EventId);
    }

    public async Task<QueueMessage> DequeueAsync(CancellationToken cancellationToken)
    {
        return await _inMemoryMessageQueue.Reader.ReadAsync(cancellationToken);
    }
}