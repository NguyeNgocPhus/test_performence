using test_peformance.Domain.Entities;

namespace test_peformance.Application.Conversations;

public interface IConversationService
{
    Task<IEnumerable<Conversation>> GetAllAsync();
    Task<Conversation?> GetByIdAsync(int id);
    Task<Conversation> CreateAsync(Conversation conversation);
    Task UpdateAsync(int id, Conversation conversation);
}
