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
        modelBuilder.ApplyConfiguration(new DepartmentConfiguration());
        modelBuilder.ApplyConfiguration(new EmployeeConfiguration());

        modelBuilder.Entity<Department>(builder =>
        {
            builder.ToTable("Department");
           
        });
        modelBuilder.Entity<Employee>(builder =>
        {
            builder.ToTable("Employee");
        });
    }

}