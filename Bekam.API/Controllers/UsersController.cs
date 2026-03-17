using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Bekam.API.Attributes.HasPermission;
using Bekam.API.Extensions;
using Bekam.Application.Abstraction.Consts;
using Bekam.Application.Abstraction.Contracts.Services.Identity;
using Bekam.Application.DTOs.Auth.Roles;
using Bekam.Application.DTOs.Users;

namespace Bekam.API.Controllers;
[Route("api/[controller]")]
[ApiController]
[EnableRateLimiting(RateLimiters.UserLimiter)]
public class UsersController(IUsersService usersService) : ControllerBase
{
    private readonly IUsersService _usersService = usersService;

    [HasPermission(Permissions.GetRoles)]
    [HttpGet("{userId}/roles")]    
    public async Task<IActionResult> GetUserRoles(string userId)
    {
        var result = await _usersService.GetUserRolesAsync(userId);
        return result.ToApiResponse();
    }

    [HasPermission(Permissions.AddRoles)]
    [HttpPost("{userId}/roles")]
    public async Task<IActionResult> AddRole(string userId, AssignUserRoleDto dto)
    {
        var result = await _usersService.AddUserToRoleAsync(userId, dto.RoleId);
        return result.IsSuccess ? NoContent() : result.ToApiResponse();
    }

    [HasPermission(Permissions.DeleteRoles)]
    [HttpDelete("{userId}/roles")]
    public async Task<IActionResult> RemoveRole(string userId, AssignUserRoleDto dto)
    {
        var result = await _usersService.RemoveUserFromRoleAsync(userId, dto.RoleId);
        return result.IsSuccess ? NoContent() : result.ToApiResponse();
    }

    [HasPermission(Permissions.GetUsers)]
    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] UserSpecParams specParams)
    {
        var result = await _usersService.GetAllAsync(specParams);
        return result.ToApiResponse();
    }

    [HasPermission(Permissions.UpdateUsers)]
    [HttpPatch("{userId}/toggle-status")]
    public async Task<IActionResult> ToggleStatus(string userId)
    {
        var result = await _usersService.ToggleStatusAsync(userId);

        return result.IsSuccess ? NoContent() : result.ToApiResponse();
    }

    [HasPermission(Permissions.UpdateUsers)]
    [HttpPatch("{userId}/toggle-lock")]
    public async Task<IActionResult> ToggleLock(string userId)
    {
        var result = await _usersService.ToggleLockAsync(userId);

        return result.IsSuccess ? NoContent() : result.ToApiResponse();
    }
}
