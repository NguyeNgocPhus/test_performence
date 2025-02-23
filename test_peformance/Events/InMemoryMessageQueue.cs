using System.Threading.Channels;

namespace test_peformance.Events;

public class InMemoryMessageQueue
{
    private readonly Channel<IIntegrationEvent> _channel = Channel.CreateUnbounded<IIntegrationEvent>();

    public ChannelWriter<IIntegrationEvent>  Writer => _channel.Writer;
    public ChannelReader<IIntegrationEvent> Reader => _channel.Reader;
 
}