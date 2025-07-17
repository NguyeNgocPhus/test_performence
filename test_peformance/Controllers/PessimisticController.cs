using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace test_peformance.Controllers;

public class PessimisticController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly ApplicationDbContext _dbContext;

    public PessimisticController(ILogger<WeatherForecastController> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    [HttpPost]
    [Route("/workItem/assign-pessimistic")]
    public async Task<IActionResult> Update([FromBody] PutDepartment req, CancellationToken cancellationToken)
    {
        var transaction = await _dbContext.Database
            .BeginTransactionAsync(System.Data.IsolationLevel.Serializable, cancellationToken);
        try
        {
            var deparment = await _dbContext.Departments.FirstOrDefaultAsync(x => x.Id == req.Id, cancellationToken);
            if (deparment == null) throw new Exception("Not found");

            deparment.Name = req.Name;
            _dbContext.Departments.Update(deparment);
            await _dbContext.SaveChangesAsync(cancellationToken);

            await Task.Delay(10000, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Ok(deparment);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
    [HttpGet]
    [Route("/workItem/assign-pessimistic/{id}")]
    public async Task<IActionResult> Select(int id, CancellationToken cancellationToken)
    {
        var deparment = await _dbContext.Departments.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return Ok(deparment);
    }
}