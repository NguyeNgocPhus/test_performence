using Microsoft.EntityFrameworkCore;
using test_peformance.Entities;
using test_peformance.Middleware;

namespace test_peformance.Data;

public class AppDbContextAccessor : IAppDbContext
{
    private readonly RywDecision _decision;
    private readonly ApplicationDbContext _master;
    private readonly ApplicationDbReplicaContext _replica;

    public AppDbContextAccessor(RywDecision decision, ApplicationDbContext master, ApplicationDbReplicaContext replica)
    {
        _decision = decision;
        _master = master;
        _replica = replica;
    }

    private DbContext Db => _decision.Role == DbRole.Master ? _master : _replica;
    public DbSet<Conversation> Conversations => Db.Set<Conversation>();

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        // Save should always go to master (defensive)
        return _master.SaveChangesAsync(ct);
    }
}