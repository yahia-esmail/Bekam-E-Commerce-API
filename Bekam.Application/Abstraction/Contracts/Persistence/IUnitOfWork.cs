using Bekam.Domain.Common;

namespace Bekam.Application.Abstraction.Contracts.Persistence;
public interface IUnitOfWork : IAsyncDisposable
{
    IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>()
        where TEntity : BaseEntity<TKey>
        where TKey : IEquatable<TKey>;

    IOrderItemRepository OrderItems { get; }
    Task<int> CompleteAsync();

}
