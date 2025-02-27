using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using test_peformance.Abstractions;
using test_peformance.Entities;
using test_peformance.Events;

namespace test_peformance.Controllers;

public class InMemoriesQueueController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IMessageQueueService _messageQueue;

    public InMemoriesQueueController(ILogger<WeatherForecastController> logger, IMessageQueueService messageQueue)
    {
        _logger = logger;
        _messageQueue = messageQueue;
    }

    [HttpPost]
    [Route("hub_event")]
    public async Task<ActionResult> HubEvent([FromBody] PutDepartment req, CancellationToken cancellationToken)
    {
        var random = new Random();
        var department = new Department()
        {
            Name = req.Name,
            Version = Guid.NewGuid(),
        };
        var queueEvent = new QueueMessage()
        {
             EventId = Guid.NewGuid().ToString(),
             EntityName = department.GetType().FullName,
             ObjEvent = JsonSerializer.Serialize(department)
        };
        await _messageQueue.EnqueueAsync(queueEvent, cancellationToken);
        return Ok("OK");
    }
    // [HttpPost]
    // [Route("complete_event")]
    // public async Task<ActionResult> CompleteEvent( CancellationToken cancellationToken)
    // {
    //     _inMemoryMessageQueue.Writer.Complete();
    //     return Ok("OK");
    // }
    //
    // [HttpGet]
    // [Route("get_event_from_queue")]
    // public async Task<ActionResult> GetEvent(CancellationToken cancellationToken)
    // {
    //     return Ok("OK");
    // }
}