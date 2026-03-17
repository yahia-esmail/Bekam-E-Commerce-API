using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Bekam.Application.Abstraction.Consts;
using Bekam.Application.Abstraction.Contracts.Persistence.DbInitializers;
using Bekam.Domain.Entities.Identity;
using Bekam.Infrastructure.Persistence._Common;

namespace Bekam.Infrastructure.Persistence._Identity;
internal class IdentityDbInitializer(IdentityDbContext _dbContext, UserManager<ApplicationUser> _userManager, RoleManager<ApplicationRole> roleManager)
        : DbInitializer(_dbContext), IIdentityDbInitializer
{
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;

    public override async Task SeedAsync()
    {
        // 1️- Seed Roles
        if (!await _roleManager.Roles.AnyAsync())
        {
            var roles = new List<ApplicationRole>
        {
            new ApplicationRole
            {
                Id = DefaultRoles.AdminRoleId,
                Name = DefaultRoles.Admin,
                NormalizedName = DefaultRoles.Admin.ToUpper(),
                CreatedBy = "System"
            },
            new ApplicationRole
            {
                Id = DefaultRoles.MemberRoleId,
                Name = DefaultRoles.Member,
                NormalizedName = DefaultRoles.Member.ToUpper(),
                IsDefault = true,
                CreatedBy = "System"
            },
            new ApplicationRole
            {
                Id = DefaultRoles.ReadOnlyAdminRoleId,
                Name = DefaultRoles.ReadOnlyAdmin,
                NormalizedName = DefaultRoles.ReadOnlyAdmin.ToUpper(),
                CreatedBy = "System"
            }
        };

            foreach (var role in roles)
                await _roleManager.CreateAsync(role);
        }

        // 2️- Seed Admin Permissions (RoleClaims)
        var adminRole = await _roleManager.FindByIdAsync(DefaultRoles.AdminRoleId);

        if (adminRole is not null)
        {
            var existingClaims = await _roleManager.GetClaimsAsync(adminRole);
            var permissions = Permissions.GetAllPermissions();

            foreach (var permission in permissions)
            {
                if (!existingClaims.Any(c => c.Type == Permissions.Type && c.Value == permission))
                {
                    await _roleManager.AddClaimAsync(adminRole,
                        new Claim(Permissions.Type, permission));
                }
            }
        }

        // Seed ReadOnlyAdmin Permissions
        var readOnlyAdminRole = await _roleManager.FindByIdAsync(DefaultRoles.ReadOnlyAdminRoleId);

        if (readOnlyAdminRole is not null)
        {
            var existingClaims = await _roleManager.GetClaimsAsync(readOnlyAdminRole);

            var permissions = Permissions
                .GetAllPermissions()
                .Where(p => p.EndsWith("Get")); // only Get permissions

            foreach (var permission in permissions)
            {
                if (!existingClaims.Any(c => c.Type == Permissions.Type && c.Value == permission))
                {
                    await _roleManager.AddClaimAsync(readOnlyAdminRole,
                        new Claim(Permissions.Type, permission));
                }
            }
        }

        // 3️- Seed Admin User
        if (!await _userManager.Users.AnyAsync())
        {
            var user = new ApplicationUser
            {
                FirstName = "Mohamed",
                LastName = "Abozied",
                UserName = "Mohamed.Abozied",
                Email = "Admin@Bekam.com",
                PhoneNumber = "01020304050",
                IsActive = true,
                EmailConfirmed = true
            };

            await _userManager.CreateAsync(user, "Admin@123");
            await _userManager.AddToRoleAsync(user, DefaultRoles.Admin);

            var ReadOnlyAdmin = new ApplicationUser
            {
                FirstName = "Mohamed",
                LastName = "Abozied",
                UserName = "Mohamed.Abozied.ReadOnlyAdmin",
                Email = "ReadOnlyAdmin@Bekam.com",
                IsActive = true,
                EmailConfirmed = true
            };

            await _userManager.CreateAsync(ReadOnlyAdmin, "ReadOnlyAdmin@123");
            await _userManager.AddToRoleAsync(ReadOnlyAdmin, DefaultRoles.ReadOnlyAdmin);
        }
    }
}