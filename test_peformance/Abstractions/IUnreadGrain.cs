using test_peformance.Entities;

namespace test_peformance.Abstractions;

public interface IUnreadGrain : IGrainWithStringKey
{
    Task UpdateUnreadConversation(UpdateUnreadConversation message);
}