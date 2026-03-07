using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test_peformance.Application.Departments;
using test_peformance.Application.DTOs;
using test_peformance.Domain.Entities;
using test_peformance.Infrastructure.Persistence;

namespace test_peformance.Presentation.Controllers;

[ApiController]
public class DepartmentController : ControllerBase
{
    private readonly ILogger<DepartmentController> _logger;
    private readonly IDepartmentService _departmentService;
    private readonly ApplicationDbContext _dbContext;

    public DepartmentController(ILogger<DepartmentController> logger, IDepartmentService departmentService, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _departmentService = departmentService;
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
    public async Task<ActionResult<Department>> GetProduct(CreateDepartmentDto req)
    {
        var userIds = new List<int>() { 4, 5 };
        for (int i = 10; i < 30; i++)
        {
            userIds.Add(i);
        }

        var departments = await (from d in _dbContext.Departments
            where userIds.Contains(d.Id)
            select d).ToListAsync();

        var departments1 = await (from d in _dbContext.Departments
            select d).ToListAsync();
        return Ok(departments1);
    }

    [HttpPost]
    [Route("create_department")]
    public async Task<ActionResult<Department>> PostProduct(CreateDepartmentDto req)
    {
        var department = await _departmentService.CreateAsync(req);
        return Ok(department);
    }

    [HttpPut("put_department/{id}")]
    public async Task<IActionResult> PutProduct(int id, UpdateDepartmentDto req)
    {
        try
        {
            var department = await _departmentService.UpdateAsync(id, req);
            return Ok(department);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogInformation(ex.Message);
            throw;
        }
    }
}
