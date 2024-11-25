namespace test_peformance.Abstractions;

public interface IMessageProvider
{
    void SendMessage<T> (T message);
}