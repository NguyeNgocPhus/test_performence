using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using test_peformance.Domain.Entities;
using test_peformance.Infrastructure.Persistence.Constants;

namespace test_peformance.Infrastructure.Persistence.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Name).IsRequired().IsUnicode();

        builder.HasOne(c => c.Department)
            .WithMany(c => c.Employees)
            .HasForeignKey(c => c.DepartmentId);

        builder.ToTable(TableNames.Employee);
    }
}
