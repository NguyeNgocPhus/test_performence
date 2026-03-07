using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using test_peformance.Application.Messages;
using test_peformance.Domain.Entities;
using test_peformance.Infrastructure.Persistence.Constants;
using test_peformance.Presentation.Hubs;

namespace test_peformance.Presentation.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = AuthScheme.Hub)]
[Route("api/[controller]")]
public class MessageController : ControllerBase
{
    private readonly IMessageService _messageService;
    private readonly IHubContext<ChatHub> _chatHub;
    private readonly IGrainFactory _grainFactory;

    public MessageController(IMessageService messageService, IHubContext<ChatHub> chatHub, IGrainFactory grainFactory)
    {
        _messageService = messageService;
        _chatHub = chatHub;
        _grainFactory = grainFactory;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Message>>> GetMessages()
        => Ok(await _messageService.GetAllAsync());

    [HttpGet("conversation/{conversationId}")]
    public async Task<ActionResult<IEnumerable<Message>>> GetMessagesByConversation(int conversationId)
        => Ok(await _messageService.GetByConversationAsync(conversationId));

    [HttpGet("{id}")]
    public async Task<ActionResult<Message>> GetMessage(int id)
    {
        var message = await _messageService.GetByIdAsync(id);
        if (message == null) return NotFound();
        return message;
    }

    [HttpPost]
    public async Task<ActionResult<Message>> CreateMessage(Message message)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "anonymous";

        try
        {
            var created = await _messageService.CreateAsync(message, userId);
            return CreatedAtAction(nameof(GetMessage), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMessage(int id, Message message)
    {
        if (id != message.Id) return BadRequest();

        try
        {
            await _messageService.UpdateAsync(id, message);
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
        {
            var existing = await _messageService.GetByIdAsync(id);
            if (existing == null) return NotFound();
            throw;
        }

        return NoContent();
    }
}
