using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using test_peformance.Entities;


namespace test_peformance;

public class ApplicationDbReplicaContext : DbContext
{
    public ApplicationDbReplicaContext([NotNull] DbContextOptions<ApplicationDbReplicaContext> options)
        : base(options)
    {
    }
    public DbSet<Conversation> Conversations { get; set; } = null!;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.OrderId).HasMaxLength(100);
            entity.Property(e => e.LastActivityAt).IsRequired();

            // One-to-many relationship with Message
            entity.HasMany(e => e.Messages)
                .WithOne(e => e.Conversation)
                .HasForeignKey(e => e.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}