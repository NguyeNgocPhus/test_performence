namespace test_peformance.Abstractions;

public interface IHelloGrain : IGrainWithStringKey
{
    Task Subscribe(IChat observer);
    Task UnSubscribe(IChat observer);
    Task SendUpdateMessage(string message);
}