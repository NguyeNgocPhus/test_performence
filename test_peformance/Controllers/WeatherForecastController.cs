using EventBus.Abstractions;
using IntegrationEventLogEF.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test_peformance.Entities;
using test_peformance.Event;

namespace test_peformance.Controllers;

[ApiController]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly IEventBus _eventBus;
    public WeatherForecastController(ILogger<WeatherForecastController> logger, ApplicationDbContext dbContext, IEventBus eventBus)
    {
        _logger = logger;
        _dbContext = dbContext;
        _eventBus = eventBus;
    }

    
    
    [HttpGet]
    [Route("test")]
    public async Task<IActionResult> Get()
    {
        try
        {
            _eventBus.Publish(new TestEvent()
            {
                Name = "Phus"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERROR publishing integration event: {IntegrationEventId} from {AppName}");
        }
        
        return Ok();
    }

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> Create()
    {

        var person = new Department()
        {
            Id = 1,
            Name = "phunn",
            // Version = Guid.NewGuid().ToByteArray()
        };
        _dbContext.Add(person);
        await _dbContext.SaveChangesAsync();
        return Ok(person);
    }
}