using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Bekam.API.Attributes.HasPermission;
using Bekam.API.Extensions;
using Bekam.Application.Abstraction.Consts;
using Bekam.Application.Abstraction.Contracts.Services.Identity;
using Bekam.Application.DTOs.Auth.Roles;

namespace Bekam.API.Controllers;
[Route("api/[controller]")]
[ApiController]
[EnableRateLimiting(RateLimiters.UserLimiter)]
public class RolesController(IRolesService rolesService) : ControllerBase
{
    private readonly IRolesService _rolesService = rolesService;

    [HasPermission(Permissions.GetRoles)]
    [HttpGet]
    [EnableRateLimiting(RateLimiters.UserLimiter)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _rolesService.GetAllAsync();
        return result.ToApiResponse();
    }

    [HasPermission(Permissions.GetPermissions)]
    [HttpGet("permissions")]
    [EnableRateLimiting(RateLimiters.UserLimiter)]
    public IActionResult GetAllPermissions()
    {
        var result = _rolesService.GetAllPermissions();
        return result.ToApiResponse();
    }

    [HasPermission(Permissions.GetRoles)]
    [HttpGet("{id}")]
    [EnableRateLimiting(RateLimiters.UserLimiter)]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _rolesService.GetByIdAsync(id);
        return result.ToApiResponse();
    }

    [HasPermission(Permissions.AddRoles)]
    [HttpPost]
    [EnableRateLimiting(RateLimiters.UserLimiter)]
    public async Task<IActionResult> Create(CreateRoleDto dto)
    {
        var result = await _rolesService.CreateAsync(dto);

        if (result.IsFailure)
            return result.ToApiResponse();

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value.Id },
            result.Value);
    }

    [HasPermission(Permissions.UpdateRoles)]
    [HttpPut("{id}")]
    [EnableRateLimiting(RateLimiters.UserLimiter)]
    public async Task<IActionResult> Update(string id, UpdateRoleDto dto)
    {
        var result = await _rolesService.UpdateAsync(id, dto);
        return result.IsSuccess ? NoContent() : result.ToApiResponse();
    }
    
    [HasPermission(Permissions.DeleteRoles)]
    [HttpDelete("{id}")]
    [EnableRateLimiting(RateLimiters.UserLimiter)]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _rolesService.DeleteAsync(id);
        return result.IsSuccess ? NoContent() : result.ToApiResponse();
    }

    [HasPermission(Permissions.AddPermissions)]
    [HttpPost("{id}/permissions")]
    [EnableRateLimiting(RateLimiters.UserLimiter)]
    public async Task<IActionResult> AddPermission(string id, [FromBody] AddPermissionDto dto)
    {
        var result = await _rolesService.AddPermissionAsync(id, dto.PermissionName);
        return result.IsSuccess ? NoContent() : result.ToApiResponse();
    }

    [HasPermission(Permissions.DeletePermissions)]
    [HttpDelete("{id}/permissions")]
    [EnableRateLimiting(RateLimiters.UserLimiter)]
    public async Task<IActionResult> RemovePermission(string id, [FromBody] RemovePermissionDto dto)
    {
        var result = await _rolesService.RemovePermissionAsync(id, dto.PermissionName);
        return result.IsSuccess ? NoContent() : result.ToApiResponse();
    }
}
