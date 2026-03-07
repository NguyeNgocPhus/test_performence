using Microsoft.EntityFrameworkCore;
using test_peformance.Domain.Entities;
using test_peformance.Infrastructure.Persistence;

namespace test_peformance.Application.Conversations;

public class ConversationService : IConversationService
{
    private readonly ApplicationDbContext _context;

    public ConversationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Conversation>> GetAllAsync()
        => await _context.Conversations.ToListAsync();

    public async Task<Conversation?> GetByIdAsync(int id)
        => await _context.Conversations.FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Conversation> CreateAsync(Conversation conversation)
    {
        conversation.LastActivityAt = DateTime.UtcNow;
        _context.Conversations.Add(conversation);
        await _context.SaveChangesAsync();
        return conversation;
    }

    public async Task UpdateAsync(int id, Conversation conversation)
    {
        conversation.LastActivityAt = DateTime.UtcNow;
        _context.Entry(conversation).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
}
