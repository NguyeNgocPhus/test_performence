using test_peformance.Abstractions;

namespace test_peformance.Grains;

public class ChatGrain : IChat
{
    public Task ReceiveMessage(string message)
    {
        Console.WriteLine(message);
        return Task.CompletedTask;
    }
}