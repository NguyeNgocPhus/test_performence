using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test_peformance.Entities;

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

    public WeatherForecastController(ILogger<WeatherForecastController> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    
    
    [HttpGet]
    [Route("test")]
    public async Task<IActionResult> Get()
    {

        var person = await _dbContext.Departments.SingleAsync(b => b.Id == 1);
        person.Name = "phunn";
        // person.Version = Guid.NewGuid().ToByteArray();
        await _dbContext.SaveChangesAsync();
        return Ok(person);
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