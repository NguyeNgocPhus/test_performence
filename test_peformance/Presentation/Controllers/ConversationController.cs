using Microsoft.AspNetCore.Mvc;
using test_peformance.Application.Conversations;
using test_peformance.Domain.Entities;

namespace test_peformance.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConversationController : ControllerBase
{
    private readonly IConversationService _conversationService;

    public ConversationController(IConversationService conversationService)
    {
        _conversationService = conversationService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Conversation>>> GetConversations()
        => Ok(await _conversationService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<Conversation>> GetConversation(int id)
    {
        var conversation = await _conversationService.GetByIdAsync(id);
        if (conversation == null) return NotFound();
        return conversation;
    }

    [HttpPost]
    public async Task<ActionResult<Conversation>> CreateConversation(Conversation conversation)
    {
        var created = await _conversationService.CreateAsync(conversation);
        return CreatedAtAction(nameof(GetConversation), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateConversation(int id, Conversation conversation)
    {
        if (id != conversation.Id) return BadRequest();

        try
        {
            await _conversationService.UpdateAsync(id, conversation);
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
        {
            var existing = await _conversationService.GetByIdAsync(id);
            if (existing == null) return NotFound();
            throw;
        }

        return NoContent();
    }
}
