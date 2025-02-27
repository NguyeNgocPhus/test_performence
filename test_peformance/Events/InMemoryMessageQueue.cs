using System.Threading.Channels;

namespace test_peformance.Events;

public class InMemoryMessageQueue
{
    private readonly Channel<QueueMessage> _channel = Channel.CreateUnbounded<QueueMessage>();

    public ChannelWriter<QueueMessage>  Writer => _channel.Writer;
    public ChannelReader<QueueMessage> Reader => _channel.Reader;
 
}