using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Bekam.Application.Abstraction.Contracts.Services;
using Bekam.Domain.Common;

namespace Bekam.Infrastructure.Persistence._Data.Interceptors;
internal class AuditInterceptor : SaveChangesInterceptor
{
    private readonly ILoggedInUserService _loggedInUserService;

    public AuditInterceptor(ILoggedInUserService loggedInUserService)
    {
        _loggedInUserService = loggedInUserService;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateEntities(DbContext? dbContext)
    {
        if (dbContext is null)
            return;

        var entries = dbContext.ChangeTracker.Entries<IBaseAuditableEntity>()
            .Where(entity => entity.State is EntityState.Added or EntityState.Modified);


        foreach (var entry in entries)
        {
            var userId = _loggedInUserService.UserId ?? "System";

            if (entry.State is EntityState.Added)
            {
                entry.Entity.CreatedBy = userId;
                entry.Entity.CreatedOn = DateTime.UtcNow;
            }
            else if (entry.State is EntityState.Modified)
            {
                entry.Entity.LastModifiedBy = userId;
                entry.Entity.LastModifiedOn = DateTime.UtcNow;

            }
        }


    }

}