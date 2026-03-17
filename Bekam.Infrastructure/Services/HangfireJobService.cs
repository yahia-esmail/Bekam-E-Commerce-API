using Hangfire;
using System.Linq.Expressions;
using Bekam.Application.Abstraction.Contracts.Services;

namespace Bekam.Infrastructure.Services;
public class HangfireJobService : IBackgroundJobService
{
    public string Enqueue(Expression<Action> job)
        => BackgroundJob.Enqueue(job);

    public string Enqueue<T>(Expression<Action<T>> job)
        => BackgroundJob.Enqueue(job);
}

