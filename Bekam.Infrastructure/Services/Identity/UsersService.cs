using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Bekam.Application.Abstraction.Contracts.Services.Identity;
using Bekam.Application.Abstraction.Errors;
using Bekam.Application.Abstraction.Results;
using Bekam.Application.DTOs.common;
using Bekam.Application.DTOs.Users;
using Bekam.Domain.Entities.Identity;
using Bekam.Infrastructure.Persistence._Identity;

namespace Bekam.Infrastructure.Services.Identity;
public class UsersService(IdentityDbContext identityDbContext, 
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager) : IUsersService
{
    private readonly IdentityDbContext _identityDbContext = identityDbContext;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;

    public async Task<Result<PaginatedResult<UserDto>>> GetAllAsync(UserSpecParams spec)
    {
        var baseQuery = _identityDbContext.Users.AsQueryable();

        if (!string.IsNullOrEmpty(spec.Search))
        {
            baseQuery = baseQuery.Where(u =>
                u.NormalizedEmail!.Contains(spec.Search) ||
                u.NormalizedUserName!.Contains(spec.Search) ||
                (u.FirstName + " " + u.LastName).ToUpper().Contains(spec.Search));
        }

        if (spec.IsActive.HasValue)
            baseQuery = baseQuery.Where(u => u.IsActive == spec.IsActive);

        if (spec.IsLocked.HasValue)
            baseQuery = baseQuery.Where(u =>
                (u.LockoutEnd.HasValue && u.LockoutEnd > DateTimeOffset.UtcNow) == spec.IsLocked);

        var totalCount = await baseQuery.CountAsync();

        var users = await (
            from u in baseQuery
            join ur in _identityDbContext.UserRoles on u.Id equals ur.UserId into userRoles
            from ur in userRoles.DefaultIfEmpty()
            join r in _identityDbContext.Roles on ur.RoleId equals r.Id into roles
            from r in roles.DefaultIfEmpty()
            group r by u into g
            select new UserDto
            {
                Id = g.Key.Id,
                UserName = g.Key.UserName!,
                FullName = g.Key.FirstName + " " + g.Key.LastName,
                Email = g.Key.Email!,
                EmailConfirmed = g.Key.EmailConfirmed,
                IsActive = g.Key.IsActive,
                IsLocked = g.Key.LockoutEnd.HasValue &&
                           g.Key.LockoutEnd > DateTimeOffset.UtcNow,
                JoinedAt = g.Key.JoinedAt,
                Roles = g.Where(x => x != null).Select(x => x.Name!).ToList()
            }
        )
        .Skip((spec.PageNumber - 1) * spec.PageSize)
        .Take(spec.PageSize)
        .ToListAsync();

        var result = new PaginatedResult<UserDto>(
            users,
            spec.PageNumber,
            spec.PageSize,
            totalCount
        );

        return Result.Success(result);
    }

    public async Task<Result> ToggleStatusAsync(string userId)
    {
        if (await _userManager.FindByIdAsync(userId) is not { } user)
            return Result.Failure(UserErrors.UserNotFound);

        user.IsActive = !user.IsActive;

        var result = await _userManager.UpdateAsync(user);

        return result.Succeeded ? Result.Success() :
             Result.Failure(UserErrors.FailedInUpdateUserStatus);
    }

    public async Task<Result> ToggleLockAsync(string userId)
    {
        if (await _userManager.FindByIdAsync(userId) is not { } user)
            return Result.Failure(UserErrors.UserNotFound);

        var isLocked =
            user.LockoutEnd.HasValue &&
            user.LockoutEnd > DateTimeOffset.UtcNow;

        if (isLocked)
        {
            user.LockoutEnd = null;
        }
        else
        {
            user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100);
        }

        var result = await _userManager.UpdateAsync(user);

        return result.Succeeded ? Result.Success() :
             Result.Failure(UserErrors.LockFailed);
    }

    public async Task<Result<IReadOnlyList<string>>> GetUserRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return Result.Failure<IReadOnlyList<string>>(UserErrors.UserNotFound);

        var roles = await _userManager.GetRolesAsync(user);

        return Result.Success<IReadOnlyList<string>>(roles.ToList());
    }

    public async Task<Result> AddUserToRoleAsync(string userId, string roleId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return Result.Failure(UserErrors.UserNotFound);

        if (await _roleManager.FindByIdAsync(roleId) is not { } role)
            return Result.Failure(RoleErrors.RoleNotFound);

        if (await _userManager.IsInRoleAsync(user, role.Name!))
            return Result.Failure(RoleErrors.RoleAlreadyAssign);

        var result = await _userManager.AddToRoleAsync(user, role.Name!);

        return result.Succeeded
            ? Result.Success()
            : Result.Failure(RoleErrors.FailedToAssignRole);
    }

    public async Task<Result> RemoveUserFromRoleAsync(string userId, string roleId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return Result.Failure(UserErrors.UserNotFound);

        if (await _roleManager.FindByIdAsync(roleId) is not { } role)
            return Result.Failure(RoleErrors.RoleNotFound);

        var result = await _userManager.RemoveFromRoleAsync(user, role.Name!);

        return result.Succeeded
            ? Result.Success()
            : Result.Failure(RoleErrors.FailedToRemoveUserRole);
    }
}
