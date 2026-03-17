using Bekam.Application.Abstraction.Results;
using Bekam.Application.DTOs.Auth.Roles;

namespace Bekam.Application.Abstraction.Contracts.Services.Identity;
public interface IRolesService
{
    Task<Result<IReadOnlyList<RoleDto>>> GetAllAsync();
    Result<IList<string>> GetAllPermissions();
    Task<Result<RoleDetailsDto>> GetByIdAsync(string id);
    Task<Result<RoleDto>> CreateAsync(CreateRoleDto dto);
    Task<Result> UpdateAsync(string id, UpdateRoleDto dto);
    Task<Result> DeleteAsync(string id);
    Task<Result> AddPermissionAsync(string roleId, string permission);
    Task<Result> RemovePermissionAsync(string roleId, string permission);    
}
