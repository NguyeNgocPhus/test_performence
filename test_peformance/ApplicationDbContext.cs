using System.Data;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using test_peformance.Configurations;
using test_peformance.Entities;
using Thinktecture;
using Thinktecture.EntityFrameworkCore;


namespace test_peformance;

public class ApplicationDbContext : DbContext, IDbDefaultSchema
{
    public string? Schema { get; }
    public ApplicationDbContext([NotNull] DbContextOptions<ApplicationDbContext> options, IDbDefaultSchema schema = null)
        : base(options)
    {
        Schema = schema?.Schema;
    }

    public DbSet<Department> Departments { get; set; }
    public DbSet<Employee> Employees { get; set; }
    
    public DbSet<IntValueEntity> IntValueEntity { get; set; }

    public IQueryable<IntValueEntity> IntValues(List<int> values)
    {
        var userIds = new DataTable();
        userIds.Columns.Add("Values", typeof(int));
        foreach (var id in values)
        {
            var row = userIds.NewRow();
            row["Values"] = id;
            userIds.Rows.Add(row);
        }

        var param = new SqlParameter("@__Values", SqlDbType.Structured)
        {
            TypeName = "__INTVALUES",
            Value = userIds
        };
        return IntValueEntity.FromSqlRaw("SELECT [Values] FROM @__Values", param);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ConfigureTempTable<int>();       
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