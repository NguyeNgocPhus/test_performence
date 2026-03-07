using Microsoft.EntityFrameworkCore;
using test_peformance.Application.DTOs;
using test_peformance.Domain.Entities;
using test_peformance.Infrastructure.Persistence;

namespace test_peformance.Application.Departments;

public class DepartmentService : IDepartmentService
{
    private readonly ApplicationDbContext _dbContext;

    public DepartmentService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Department>> GetAllAsync()
    {
        return await _dbContext.Departments.ToListAsync();
    }

    public async Task<Department> CreateAsync(CreateDepartmentDto dto)
    {
        var department = new Department
        {
            Name = dto.Name,
            Version = Guid.NewGuid()
        };
        _dbContext.Departments.Add(department);
        await _dbContext.SaveChangesAsync();
        return department;
    }

    public async Task<Department> UpdateAsync(int id, UpdateDepartmentDto dto)
    {
        var department = await _dbContext.Departments.FirstOrDefaultAsync(x => x.Id == id);
        if (department == null) throw new Exception("Not found");

        department.Name = dto.Name;
        department.Version = Guid.NewGuid();
        _dbContext.Departments.Update(department);
        await _dbContext.SaveChangesAsync();
        return department;
    }
}
