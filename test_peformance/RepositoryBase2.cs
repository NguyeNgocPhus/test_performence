using System.Linq.Expressions;
using test_peformance.Abstractions;
using test_peformance.Entities;

namespace test_peformance;

public class RepositoryBase2<TEntity,TKey> :  IRepositoryBase<TEntity, TKey>, IDisposable
    where TEntity : BaseEntity
{
    public Task<TEntity> FindByIdAsync(TKey id, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includeProperties)
    {
        throw new NotImplementedException();
    }

    public Task<TEntity> FindSingleAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includeProperties)
    {
        throw new NotImplementedException();
    }

    public IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>>? predicate = null, params Expression<Func<TEntity, object>>[] includeProperties)
    {
        throw new NotImplementedException();
    }

    public void Add(TEntity entity)
    {
        throw new NotImplementedException();
    }

    public void Update(TEntity entity)
    {
        throw new NotImplementedException();
    }

    public void Remove(TEntity entity)
    {
        throw new NotImplementedException();
    }

    public void RemoveMultiple(List<TEntity> entities)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}