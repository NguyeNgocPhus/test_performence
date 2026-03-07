using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using test_peformance.Domain.Entities;
using test_peformance.Infrastructure.Persistence.Constants;

namespace test_peformance.Infrastructure.Persistence.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Name).IsRequired().IsUnicode();
        builder.ToTable(TableNames.Department);
    }
}
