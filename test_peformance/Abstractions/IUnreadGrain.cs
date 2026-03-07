using test_peformance.Domain.Entities;

namespace test_peformance.Abstractions;

public interface IUnreadGrain : IGrainWithStringKey
{
    Task UpdateUnreadConversation(UpdateUnreadConversation message);
}
