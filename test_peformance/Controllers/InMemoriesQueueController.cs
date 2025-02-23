using Microsoft.AspNetCore.Mvc;
using test_peformance.Abstractions;
using test_peformance.Events;

namespace test_peformance.Controllers;

public class InMemoriesQueueController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly IEventBus _eventBus;
    private readonly InMemoryMessageQueue _inMemoryMessageQueue;
    public InMemoriesQueueController(ILogger<WeatherForecastController> logger, ApplicationDbContext dbContext,
        IEventBus eventBus, InMemoryMessageQueue inMemoryMessageQueue)
    {
        _logger = logger;
        _dbContext = dbContext;
        _eventBus = eventBus;
        _inMemoryMessageQueue = inMemoryMessageQueue;
    }

    [HttpPost]
    [Route("hub_event")]
    public async Task<ActionResult> HubEvent([FromBody] PutDepartment req, CancellationToken cancellationToken)
    {
        Random random = new Random();
        var dataEvent = new HubEvent()
        {
            EventId = req.Id.ToString(),
            Name = req.Name,
            Password = random.Next(1, 100).ToString(),
        };
        
        // _inMemoryMessageQueue.AddList(dataEvent);
        await _eventBus.PublishAsync(dataEvent, cancellationToken);
        return Ok("OK");
    }
    [HttpPost]
    [Route("complete_event")]
    public async Task<ActionResult> CompleteEvent( CancellationToken cancellationToken)
    {
        _inMemoryMessageQueue.Writer.Complete();
        return Ok("OK");
    }

    [HttpGet]
    [Route("get_event_from_queue")]
    public async Task<ActionResult> GetEvent(CancellationToken cancellationToken)
    {

        await _eventBus.GetAllEventAsync(cancellationToken);
        return Ok("OK");
    }
}