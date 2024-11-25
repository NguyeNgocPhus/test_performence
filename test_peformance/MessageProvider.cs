using test_peformance.Abstractions;

namespace test_peformance;

public class MessageProvider : IMessageProvider
{
    
    public void SendMessage<T>(T message)
    {
        throw new NotImplementedException();
    }
}