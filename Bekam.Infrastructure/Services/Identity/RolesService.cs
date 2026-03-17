using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Bekam.Application.Abstraction.Consts;
using Bekam.Application.Abstraction.Contracts.Services.Identity;
using Bekam.Application.Abstraction.Errors;
using Bekam.Application.Abstraction.Results;
using Bekam.Application.DTOs.Auth.Roles;
using Bekam.Domain.Entities.Identity;

namespace Bekam.Infrastructure.Services.Identity;
internal class RolesService(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager) : IRolesService
{
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<Result<RoleDto>> CreateAsync(CreateRoleDto dto)
    {
        var exists = await _roleManager.RoleExistsAsync(dto.Name);
        if (exists)
            return Result.Failure<RoleDto>(RoleErrors.RoleAlreadyExists);

        var role = new ApplicationRole()
        {
            Name = dto.Name
        };

        var result = await _roleManager.CreateAsync(role);

        if (!result.Succeeded)
            return Result.Failure<RoleDto>(RoleErrors.FailedToCreate);

        return Result.Success(new RoleDto
        {
            Id = role.Id,
            Name = role.Name!
        });
    }

    public async Task<Result> DeleteAsync(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role is null)
            return Result.Failure(RoleErrors.RoleNotFound);

        var result = await _roleManager.DeleteAsync(role);

        return result.Succeeded
            ? Result.Success()
            : Result.Failure(RoleErrors.FailedToDelete);
    }

    public async Task<Result<IReadOnlyList<RoleDto>>> GetAllAsync()
    {
        var roles = await _roleManager.Roles
        .Select(r => new RoleDto
        {
            Id = r.Id,
            Name = r.Name!
        })
        .ToListAsync();

        return Result.Success<IReadOnlyList<RoleDto>>(roles);
    }

    public async Task<Result<RoleDetailsDto>> GetByIdAsync(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);

        if (role is null)
            return Result.Failure<RoleDetailsDto>(RoleErrors.RoleNotFound);

        var claims = await _roleManager.GetClaimsAsync(role);

        var dto = new RoleDetailsDto
        {
            Id = role.Id,
            Name = role.Name!,
            Permissions = claims
                .Where(c => c.Type == Permissions.Type)
                .Select(c => c.Value)
                .ToList()
        };

        return Result.Success(dto);
    }

    public async Task<Result> UpdateAsync(string id, UpdateRoleDto dto)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role is null)
            return Result.Failure(RoleErrors.RoleNotFound);

        role.Name = dto.Name;

        var result = await _roleManager.UpdateAsync(role);

        return result.Succeeded
            ? Result.Success()
            : Result.Failure(RoleErrors.FailedToUpdate);
    }

    public async Task<Result> AddPermissionAsync(string roleId, string permission)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role is null)
            return Result.Failure(RoleErrors.RoleNotFound);

        if(await _roleManager.GetClaimsAsync(role)
            .ContinueWith(t => t.Result.Any(c =>
                c.Type == Permissions.Type && c.Value == permission)))
        {
            return Result.Failure(RoleErrors.PermissionAlreadyExists);
        }

        var claim = new Claim(Permissions.Type, permission);

        var result = await _roleManager.AddClaimAsync(role, claim);

        return result.Succeeded
            ? Result.Success()
            : Result.Failure(RoleErrors.FailedToAddPermission);
    }

    public async Task<Result> RemovePermissionAsync(string roleId, string permission)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role is null)
            return Result.Failure(RoleErrors.RoleNotFound);

        var claims = await _roleManager.GetClaimsAsync(role);

        var claim = claims.FirstOrDefault(c =>
            c.Type == Permissions.Type && c.Value == permission);

        if (claim is null)
            return Result.Failure(RoleErrors.PermissionNotFound);

        var result = await _roleManager.RemoveClaimAsync(role, claim);

        return result.Succeeded
            ? Result.Success()
            : Result.Failure(RoleErrors.FailedToRemovePermission);
    }

    public Result<IList<string>> GetAllPermissions()
    {
        return Result.Success(Permissions.GetAllPermissions())!;
    }
}
