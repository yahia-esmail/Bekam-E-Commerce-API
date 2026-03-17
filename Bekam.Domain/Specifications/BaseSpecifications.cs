using System.Linq.Expressions;
using Bekam.Domain.Common;

namespace Bekam.Domain.Specifications;
public abstract class BaseSpecifications<TEntity, TKey>
    : ISpecifications<TEntity, TKey>
    where TEntity : BaseEntity<TKey>
    where TKey : IEquatable<TKey>
{
    public Expression<Func<TEntity, bool>>? Criteria { get; protected set; }

    protected readonly List<Expression<Func<TEntity, object>>> _includes = new();
    public IReadOnlyList<Expression<Func<TEntity, object>>> Includes => _includes;

    public Expression<Func<TEntity, object>>? OrderBy { get; protected set; }
    public Expression<Func<TEntity, object>>? OrderByDescending { get; protected set; }

    public int? Skip { get; protected set; }
    public int? Take { get; protected set; }

    public bool IsPagingEnabled { get; protected set; } = true;

    protected BaseSpecifications() { }
    protected BaseSpecifications(Expression<Func<TEntity, bool>> criteria)
        => Criteria = criteria;

    protected BaseSpecifications(TKey id)
    {
        Criteria = E => E.Id!.Equals(id);
    }

    protected void AddInclude(Expression<Func<TEntity, object>> include)
        => _includes.Add(include);

    protected void ApplyOrderBy(Expression<Func<TEntity, object>> orderBy)
        => OrderBy = orderBy;

    protected void ApplyOrderByDescending(Expression<Func<TEntity, object>> orderByDesc)
        => OrderByDescending = orderByDesc;

    protected void ApplyPaging(int skip, int take)
    {
        IsPagingEnabled = true;
        Skip = skip;
        Take = take;
    }

    public void DisablePaging() // to get totalCount with filtration
    {
        IsPagingEnabled = false;
        Skip = null;
        Take = null;
    }
}
