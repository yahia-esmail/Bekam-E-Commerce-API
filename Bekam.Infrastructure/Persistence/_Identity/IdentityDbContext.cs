using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Bekam.Domain.Entities.Identity;

namespace Bekam.Infrastructure.Persistence._Identity;
public class IdentityDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
            : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(
            typeof(AssemblyInformation).Assembly,
            t => t.Namespace!.Contains("Persistence._Identity.Configs")
        );
    }

}
