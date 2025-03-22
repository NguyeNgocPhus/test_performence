using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test_peformance.Entities;

namespace test_peformance.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessageController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public MessageController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Message>>> GetMessages()
    {
        return await _context.Messages
            .Include(m => m.Conversation)
            .ToListAsync();
    }

    [HttpGet("conversation/{conversationId}")]
    public async Task<ActionResult<IEnumerable<Message>>> GetMessagesByConversation(int conversationId)
    {
        return await _context.Messages
            .Where(m => m.ConversationId == conversationId)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Message>> GetMessage(int id)
    {
        var message = await _context.Messages
            .Include(m => m.Conversation)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (message == null)
        {
            return NotFound();
        }

        return message;
    }

    [HttpPost]
    public async Task<ActionResult<Message>> CreateMessage(Message message)
    {
        message.CreatedAt = DateTime.UtcNow;
        
        // Update conversation's LastActivityAt
        var conversation = await _context.Conversations.FindAsync(message.ConversationId);
        if (conversation == null)
        {
            return BadRequest("Invalid ConversationId");
        }
        
        conversation.LastActivityAt = DateTime.UtcNow;
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMessage), new { id = message.Id }, message);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMessage(int id, Message message)
    {
        if (id != message.Id)
        {
            return BadRequest();
        }

        _context.Entry(message).State = EntityState.Modified;
        message.CreatedAt = DateTime.UtcNow; // You might want to keep the original CreatedAt

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Messages.AnyAsync(m => m.Id == id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }
} 