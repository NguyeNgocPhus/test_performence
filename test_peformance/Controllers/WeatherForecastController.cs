using EventBus.Abstractions;
using Microsoft.AspNetCore.Mvc;
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
    private readonly IWebHostEnvironment _env;
    private readonly string _uploadPath;
    public WeatherForecastController(IWebHostEnvironment env,ILogger<WeatherForecastController> logger, ApplicationDbContext dbContext, IEventBus eventBus)
    {
        _logger = logger;
        _dbContext = dbContext;
        _eventBus = eventBus;
        _env = env;
        _uploadPath = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads");

        if (!Directory.Exists(_uploadPath))
            Directory.CreateDirectory(_uploadPath);
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
    [Route("file")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is empty or missing.");

        var fileName = Path.GetFileName(file.FileName);
        var savePath = Path.Combine(_uploadPath, fileName);

        await using (var stream = new FileStream(savePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";
        return Ok(new { file = fileUrl });
    }
}