using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test_peformance.Entities;

namespace test_peformance.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConversationController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ConversationController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Conversation>>> GetConversations()
    {
        return await _context.Conversations
            .Include(c => c.Messages)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Conversation>> GetConversation(int id)
    {
        var conversation = await _context.Conversations
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (conversation == null)
        {
            return NotFound();
        }

        return conversation;
    }

    [HttpPost]
    public async Task<ActionResult<Conversation>> CreateConversation(Conversation conversation)
    {
        conversation.LastActivityAt = DateTime.UtcNow;
        _context.Conversations.Add(conversation);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetConversation), new { id = conversation.Id }, conversation);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateConversation(int id, Conversation conversation)
    {
        if (id != conversation.Id)
        {
            return BadRequest();
        }

        conversation.LastActivityAt = DateTime.UtcNow;
        _context.Entry(conversation).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Conversations.AnyAsync(c => c.Id == id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }
} 