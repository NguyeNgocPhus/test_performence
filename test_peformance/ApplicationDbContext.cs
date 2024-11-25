using Microsoft.EntityFrameworkCore;
using test_peformance.Configurations;
using test_peformance.Entities;

namespace test_peformance;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    
    }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Employee> Employees { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var departmentId = 1;
        modelBuilder.ApplyConfiguration(new DepartmentConfiguration());
        modelBuilder.ApplyConfiguration(new EmployeeConfiguration());

        modelBuilder.Entity<Department>(builder =>
        {
            builder.ToTable("Department");
            builder.HasData(new Department()
            {
                Name = "phunn",
                Id = departmentId,
            });
        });
        modelBuilder.Entity<Employee>(builder =>
        {
            builder.ToTable("Employee");
            var employees = Enumerable.Range(1, 100).Select(x => new Employee()
            {
                Id = x,
                DepartmentId = departmentId,
                Name = $"Employee_{x}",
                Salary = 10
            });
            
            builder.HasData(employees);
        });
    }

}