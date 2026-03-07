using Microsoft.EntityFrameworkCore;
using test_peformance.Domain.Entities;
using test_peformance.Infrastructure.Persistence;

namespace test_peformance.Application.Messages;

public class MessageService : IMessageService
{
    private readonly ApplicationDbContext _context;

    public MessageService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Message>> GetAllAsync()
        => await _context.Messages.Include(m => m.Conversation).ToListAsync();

    public async Task<IEnumerable<Message>> GetByConversationAsync(int conversationId)
        => await _context.Messages.Where(m => m.ConversationId == conversationId).ToListAsync();

    public async Task<Message?> GetByIdAsync(int id)
        => await _context.Messages.Include(m => m.Conversation).FirstOrDefaultAsync(m => m.Id == id);

    public async Task<Message> CreateAsync(Message message, string userId)
    {
        var conversation = await _context.Conversations.FindAsync(message.ConversationId);
        if (conversation == null)
            throw new InvalidOperationException("Invalid ConversationId");

        message.CreatedAt = DateTime.UtcNow;
        message.CreatedBy = userId;
        conversation.LastActivityAt = DateTime.UtcNow;

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
        return message;
    }

    public async Task UpdateAsync(int id, Message message)
    {
        _context.Entry(message).State = EntityState.Modified;
        message.CreatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }
}
