using Microsoft.AspNetCore.Mvc;

namespace test_peformance.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private static int _items = 0; // Tài nguyên chung
    private readonly ILogger<ItemsController> _logger;
    // Tạo một SemaphoreSlim duy nhất cho cả hai API
    private static SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1); // Chỉ cho phép 1 request cùng lúc
    
    public ItemsController(ILogger<ItemsController> logger)
    {
        _logger = logger;
    }

    [HttpGet("increment")]
    public async Task<IActionResult> AddItem()
    {
        await _semaphore.WaitAsync();
        try
        {
            _items += 1;
            _logger.LogInformation($"increment {_items} items");
            return Ok("Item increment");
        } finally
        {
            _semaphore.Release(); // Giải phóng quyền truy cập
        }
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
        } finally
        {
            _semaphore.Release(); // Giải phóng quyền truy cập
        }
    }
}