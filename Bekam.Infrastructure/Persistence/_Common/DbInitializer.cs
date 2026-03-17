using Microsoft.EntityFrameworkCore;
using Bekam.Application.Abstraction.Contracts.Persistence.DbInitializers;

namespace Bekam.Infrastructure.Persistence._Common;
internal abstract class DbInitializer(DbContext _dbContext) : IDbInitializer
{
    public async Task InitializeAsync()
    {
        var pendingMigrations = await _dbContext.Database.GetPendingMigrationsAsync();

        if (pendingMigrations.Any())
            await _dbContext.Database.MigrateAsync(); // Update-Database
    }

    public abstract Task SeedAsync();
}
