using test_peformance.Domain.Abstractions;

namespace test_peformance.Infrastructure.Persistence.Repositories;

public class EFUnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;

    public EFUnitOfWork(ApplicationDbContext dbContext)
        => _dbContext = dbContext;

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
        => await _dbContext.DisposeAsync();
}
