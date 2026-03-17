using Bekam.Application.Abstraction.Results;
using Bekam.Application.DTOs.common;
using Bekam.Application.DTOs.Users;

namespace Bekam.Application.Abstraction.Contracts.Services.Identity;
public interface IUsersService
{
    Task<Result<PaginatedResult<UserDto>>> GetAllAsync(UserSpecParams specParams);
    Task<Result> ToggleStatusAsync(string userId);
    Task<Result> ToggleLockAsync(string userId);
    Task<Result<IReadOnlyList<string>>> GetUserRolesAsync(string userId);
    Task<Result> AddUserToRoleAsync(string userId, string roleName);
    Task<Result> RemoveUserFromRoleAsync(string userId, string roleName);
}
