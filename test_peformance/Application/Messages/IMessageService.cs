using test_peformance.Domain.Entities;

namespace test_peformance.Application.Messages;

public interface IMessageService
{
    Task<IEnumerable<Message>> GetAllAsync();
    Task<IEnumerable<Message>> GetByConversationAsync(int conversationId);
    Task<Message?> GetByIdAsync(int id);
    Task<Message> CreateAsync(Message message, string userId);
    Task UpdateAsync(int id, Message message);
}
