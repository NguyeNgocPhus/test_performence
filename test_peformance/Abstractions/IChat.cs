namespace test_peformance.Abstractions;

public interface IChat: IGrainObserver
{
    Task ReceiveMessage(string message);
}