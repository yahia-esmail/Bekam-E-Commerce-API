using System.Security.Claims;
using Bekam.Application.Abstraction.Results;
using Bekam.Application.DTOs.Auth.change_password;
using Bekam.Application.DTOs.Users;

namespace Bekam.Application.Abstraction.Contracts.Services.Identity;

public interface IAccountService
{
    Task<Result> ChangePasswordAsync(ClaimsPrincipal userPrincipal, ChangePasswordRequest req);
    Task<Result<UserDto>> GetProfile(string userId);

}
