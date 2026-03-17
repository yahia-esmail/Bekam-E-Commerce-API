using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using Bekam.Application.Abstraction.Contracts.Persistence;
using Bekam.Domain.Common;
using Bekam.Infrastructure.Persistence._Data;
using Bekam.Infrastructure.Persistence.GenericRepository;
using Bekam.Infrastructure.Persistence.Repositories;

namespace Bekam.Infrastructure.Persistence.UnitOfWork;
public class UnitOfWork(AppDbContext dbContext) : IUnitOfWork
{

    private readonly ConcurrentDictionary<Type, object> _repositories = new();

    public IOrderItemRepository OrderItems => new OrderItemRepository(dbContext);

    public IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>()
        where TEntity : BaseEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        return (IGenericRepository<TEntity, TKey>)_repositories.GetOrAdd(typeof(TEntity), _ => new GenericRepository<TEntity, TKey>(dbContext));
    }
    public async Task<int> CompleteAsync()
            => await dbContext.SaveChangesAsync();

    public async ValueTask DisposeAsync()
        => await dbContext.DisposeAsync();

}
