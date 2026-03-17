using Microsoft.EntityFrameworkCore;
using Bekam.Application.Abstraction.Contracts.Persistence;
using Bekam.Domain.Specifications;
using Bekam.Domain.Common;
using Bekam.Domain.Specifications;
using Bekam.Infrastructure.Persistence._Data;
using Bekam.Infrastructure.Persistence.Specifications;
using System.Linq.Expressions;

namespace Bekam.Infrastructure.Persistence.GenericRepository;
internal class GenericRepository<TEntity, TKey>(AppDbContext _dbContext) : IGenericRepository<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
        where TKey : IEquatable<TKey>
{
    public async Task<TEntity?> GetByIdAsync(TKey id)
        => await _dbContext.Set<TEntity>().FindAsync(id);

    public async Task<TEntity?> GetAsync(ISpecifications<TEntity, TKey> spec)
    => await ApplySpecifications(spec).FirstOrDefaultAsync();
    public async Task<IReadOnlyList<TEntity>> GetAllAsync(bool withTracking = false)
            => withTracking ? await _dbContext.Set<TEntity>().ToListAsync()
        : await _dbContext.Set<TEntity>().AsNoTracking().ToListAsync();

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(ISpecifications<TEntity, TKey> spec, bool withTracking = false)
         => withTracking ? await ApplySpecifications(spec).ToListAsync()
        : await ApplySpecifications(spec).AsNoTracking().ToListAsync();

    public async Task<int> GetCountAsync(ISpecifications<TEntity, TKey> spec)
        => await ApplySpecifications(spec).CountAsync();

    public async Task AddAsync(TEntity entity)
        => await _dbContext.Set<TEntity>().AddAsync(entity);

    public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        => await _dbContext.Set<TEntity>().AddRangeAsync(entities);

    public void Update(TEntity entity)
        => _dbContext.Set<TEntity>().Update(entity);

    public void Delete(TEntity entity)
        => _dbContext.Set<TEntity>().Remove(entity);

    private IQueryable<TEntity> ApplySpecifications(ISpecifications<TEntity, TKey> spec)
           => SpecificationsEvaluator<TEntity, TKey>.GetQuery(_dbContext.Set<TEntity>(), spec);

    public IQueryable<TEntity> AsQueryable(ISpecifications<TEntity, TKey> spec) // to make projection easily
    {
        return ApplySpecifications(spec).AsNoTracking();
    }

    public Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return _dbContext.Set<TEntity>().AnyAsync(predicate);
    }
}