using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test_peformance.Entities;
using Thinktecture;

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

    [HttpGet]
    [Route("create_tb")]
    public async Task<ActionResult<Department>> CreateTable()
    {
        var userIds = new DataTable();
        userIds.Columns.Add("Values", typeof(int));
        userIds.Columns.Add("Id", typeof(int));
        userIds.Columns.Add("Name", typeof(int));

        var c = new BulkData();
        await c.CreateTable(userIds, "TableTest");
        return Ok("OK");
    }

    [HttpGet]
    [Route("get_department")]
    public async Task<ActionResult<Department>> GetProduct(CreateDepartment req)
    {
        var userIds = new List<int>() { 4, 5 };
        for (int i = 10; i < 30; i++)
        {
            userIds.Add(i);
        }
        // var tempTableQuery = await _dbContext.BulkInsertValuesIntoTempTableAsync(userIds);

        var departments = await (from d in _dbContext.Departments
            where userIds.Contains(d.Id)
            select d).ToListAsync();


        var departments1 = await (from d in _dbContext.Departments
            join i in _dbContext.IntValues(userIds) on d.Id equals i.Values 
            select d).ToListAsync();
        return Ok(departments1);
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