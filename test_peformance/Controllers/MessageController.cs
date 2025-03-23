using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using test_peformance.Contants;
using test_peformance.Entities;

namespace test_peformance.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = AuthScheme.Hub)]
[Route("api/[controller]")]
public class MessageController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<ChatHub> _chatHub;
    public MessageController(ApplicationDbContext context, IHubContext<ChatHub> chatHub)
    {
        _context = context;
        _chatHub = chatHub;
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
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "anonymous";    
        conversation.LastActivityAt = DateTime.UtcNow;
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
        await _chatHub.Clients.User("userId").SendAsync("ReceiveMessage","cccccc");
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