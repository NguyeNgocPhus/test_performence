using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test_peformance.Entities;

namespace test_peformance.Controllers;

[ApiController]
public class DepartmentController : ControllerBase
{
    
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly ApplicationDbContext _dbContext;

    public DepartmentController(ILogger<WeatherForecastController> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    [HttpPost]
    [Route("create_department")]
    public async Task<ActionResult<Department>> PostProduct(CreateDepartment req)
    {
        if (_dbContext.Departments is null)
        {
            return Problem("Entity set 'ProductsDbContext.Product'  is null.");
        }

        var department = new Department()
        {
                Name = req.Name,
                Version = Guid.NewGuid()
        };
        _dbContext.Departments.Add(department);
        await _dbContext.SaveChangesAsync();
        return Ok(department);
    }
    [HttpPut("put_department/{id}")]
    public async Task<IActionResult> PutProduct(int id, PutDepartment req)
    {

        var department = await _dbContext.Departments.FirstOrDefaultAsync(x => x.Id == id);
        if (department == null) throw new Exception("Not found");
        
        department.Name = req.Name;
        _dbContext.Departments.Update(department);
        
        department.Version = Guid.NewGuid();
        try
        {
            
            await _dbContext.SaveChangesAsync();
            return Ok(department);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogInformation(ex.Message);
            throw;
        }
       
    }
}

public class CreateDepartment
{
    public string Name { get; set; }
}
public class PutDepartment
{
    public int Id { get; set; }
    public string Name { get; set; }
}