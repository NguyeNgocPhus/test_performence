using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using test_peformance.Abstractions;
using test_peformance.Grains;

namespace test_peformance.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private static int _items = 0; // Tài nguyên chung

    private readonly ILogger<ItemsController> _logger;
    private readonly IClusterClient _client;
    private readonly IGrainFactory _grainFactory;

    // Tạo một SemaphoreSlim duy nhất cho cả hai API
    private static SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1); // Chỉ cho phép 1 request cùng lúc

    public ItemsController(ILogger<ItemsController> logger, IClusterClient client, IGrainFactory grainFactory)
    {
        _logger = logger;
        _client = client;
        _grainFactory = grainFactory;
    }

    [HttpGet("increment")]
    public async Task<IActionResult> AddItem([FromQuery] string productId)
    {
        var grain = _client.GetGrain<IProductGrain>(productId);
        var result = await grain.ReturnStateAsync();
        return Ok(result);
    }

    [HttpGet("decrement")]
    public async Task<IActionResult> GetItems()
    {
        try
        {
            await _semaphore.WaitAsync();
            _items -= 1;
            _logger.LogInformation($"decrement {_items} items");
            return Ok("Item decrement");
        }
        finally
        {
            _semaphore.Release(); // Giải phóng quyền truy cập
        }
    }

    [HttpGet("hello")]
    public async Task<IActionResult> Hello([FromQuery] string message)
    {
        var grain = _grainFactory.GetGrain<IHelloGrain>(String.Empty);
        await grain.SendUpdateMessage(message);
        return Ok();
    }
}