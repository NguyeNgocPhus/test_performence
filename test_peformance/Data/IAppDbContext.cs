using Microsoft.EntityFrameworkCore;
using test_peformance.Entities;

namespace test_peformance.Data;

public interface IAppDbContext
{
    DbSet<Conversation> Conversations { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}