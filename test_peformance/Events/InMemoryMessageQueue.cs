using System.Threading.Channels;

namespace test_peformance.Events;

public class InMemoryMessageQueue
{
    private readonly Channel<IIntegrationEvent> _channel = Channel.CreateUnbounded<IIntegrationEvent>();
    private List<IIntegrationEvent> abc = new List<IIntegrationEvent>();
    public ChannelWriter<IIntegrationEvent>  Writer => _channel.Writer;
    public ChannelReader<IIntegrationEvent> Reader => _channel.Reader;

    public void AddList(IIntegrationEvent integrationEvent)
    {
        var a = new SemaphoreSlim(1);
        abc.Add(integrationEvent);
    }
    public void GetList()
    {
        var a = abc.Last();
        Console.WriteLine($"I am {a.EventId}");
    }
    
}