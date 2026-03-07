using test_peformance.Application.DTOs;
using test_peformance.Domain.Entities;

namespace test_peformance.Application.Departments;

public interface IDepartmentService
{
    Task<IEnumerable<Department>> GetAllAsync();
    Task<Department> CreateAsync(CreateDepartmentDto dto);
    Task<Department> UpdateAsync(int id, UpdateDepartmentDto dto);
}
