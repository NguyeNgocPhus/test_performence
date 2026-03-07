using Microsoft.AspNetCore.Mvc;
using test_peformance.Application.Cronjob;

namespace test_peformance.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CronjobController : ControllerBase
{
    private readonly ICronjobService _cronjobService;

    public CronjobController(ICronjobService cronjobService)
    {
        _cronjobService = cronjobService;
    }

    [HttpGet]
    public async Task<ActionResult> StartJob()
    {
        var result = await _cronjobService.RunAsync();
        return Ok(result);
    }
}
