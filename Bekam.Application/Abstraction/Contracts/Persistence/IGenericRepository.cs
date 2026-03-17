using Bekam.Domain.Specifications;
using Bekam.Domain.Common;
using System.Linq.Expressions;

namespace Bekam.Application.Abstraction.Contracts.Persistence;
public interface IGenericRepository<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
        where TKey : IEquatable<TKey>
{
    Task<TEntity?> GetByIdAsync(TKey id);
    Task<TEntity?> GetAsync(ISpecifications<TEntity, TKey> spec);
    Task<IReadOnlyList<TEntity>> GetAllAsync(bool withTracking = false);
    Task<IReadOnlyList<TEntity>> GetAllAsync(ISpecifications<TEntity, TKey> spec, bool withTracking = false);
    IQueryable<TEntity> AsQueryable(ISpecifications<TEntity, TKey> spec);
    Task<int> GetCountAsync(ISpecifications<TEntity, TKey> spec);
    Task AddAsync(TEntity entity);
    Task AddRangeAsync(IEnumerable<TEntity> entities);
    void Update(TEntity entity);
    void Delete(TEntity entity);
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);
}