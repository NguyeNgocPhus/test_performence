using test_peformance.Infrastructure.Messaging.EventBus;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using test_peformance.Events;
using test_peformance.Infrastructure.Persistence;

namespace test_peformance.Presentation.Controllers;

[ApiController]
public class WeatherForecastController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IEventBus _eventBus;
    private readonly IWebHostEnvironment _env;
    private readonly string _uploadPath;

    public WeatherForecastController(IWebHostEnvironment env, ILogger<WeatherForecastController> logger, ApplicationDbContext dbContext, IEventBus eventBus)
    {
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
            var traceId = HttpContext.TraceIdentifier;
            Log.Information("Publishing integration event: {IntegrationEventId} from {AppName}", Guid.NewGuid(), _env.ApplicationName);

            _eventBus.Publish(new TestEvent()
            {
                Name = "Phus",
                TraceId = traceId
            });
            Log.Information("Publishing integration event success");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "ERROR publishing integration event: {IntegrationEventId} from {AppName}");
        }

        return Ok();
    }

    [HttpGet]
    [Route("test_order")]
    public async Task<IActionResult> GetOrder()
    {
        try
        {
            var traceId = HttpContext.TraceIdentifier;
            Log.Information("Publishing integration event: {IntegrationEventId} from {AppName}", Guid.NewGuid(), _env.ApplicationName);

            _eventBus.Publish(new OrderEvent()
            {
                Name = "Phus",
                Price = 1m,
                TraceId = traceId
            });
            Log.Information("Publishing integration event success");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "ERROR publishing integration event: {IntegrationEventId} from {AppName}");
        }

        return Ok();
    }

    [HttpPost]
    [Route("file")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        Log.Information("Uploading file {File}", file.FileName);
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
