using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using test_peformance.Domain.Entities;
using test_peformance.Infrastructure.Persistence.Configurations;
using Thinktecture;
using Thinktecture.EntityFrameworkCore;

namespace test_peformance.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext([NotNull] DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Department> Departments { get; set; }
    public DbSet<Employee> Employees { get; set; }

    public DbSet<IntValueEntity> IntValueEntity { get; set; }

    public DbSet<Conversation> Conversations { get; set; } = null!;
    public DbSet<Message> Messages { get; set; } = null!;
    public DbSet<SystemConfig> SystemConfig { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ConfigureTempTable<int>();
        modelBuilder.ApplyConfiguration(new DepartmentConfiguration());
        modelBuilder.ApplyConfiguration(new EmployeeConfiguration());

        modelBuilder.Entity<Department>(builder => { builder.ToTable("Department"); });
        modelBuilder.Entity<Employee>(builder => { builder.ToTable("Employee"); });

        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.OrderId).HasMaxLength(100);
            entity.Property(e => e.LastActivityAt).IsRequired();

            entity.HasMany(e => e.Messages)
                .WithOne(e => e.Conversation)
                .HasForeignKey(e => e.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).HasMaxLength(255);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.ConversationId).IsRequired();
        });
    }
}
