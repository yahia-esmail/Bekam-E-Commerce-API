using System.Linq.Expressions;

namespace Bekam.Application.Abstraction.Contracts.Services;
public interface IBackgroundJobService
{
    string Enqueue(Expression<Action> job);
    string Enqueue<T>(Expression<Action<T>> job);
}

