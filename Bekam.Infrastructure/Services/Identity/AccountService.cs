using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Bekam.Application.Abstraction.Contracts.Services.Identity;
using Bekam.Application.Abstraction.Errors;
using Bekam.Application.Abstraction.Results;
using Bekam.Application.DTOs.Auth.change_password;
using Bekam.Application.DTOs.Users;
using Bekam.Domain.Entities.Identity;

namespace Bekam.Infrastructure.Services.Identity;
public class AccountService(UserManager<ApplicationUser> userManager) : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<Result> ChangePasswordAsync(ClaimsPrincipal userPrincipal, ChangePasswordRequest req)
    {
        if(await _userManager.GetUserAsync(userPrincipal) is not { } user)
            return Result.Failure(UserErrors.UserNotFound);

        if (user.UserName == "Mohamed.Abozied.ReadOnlyAdmin")
            return Result.Failure(UserErrors.ActionNotAllow);

        var result = await _userManager.ChangePasswordAsync(user, req.CurrentPassword, req.NewPassword);

        if (result.Succeeded)
            return Result.Success();

        return Result.Failure(IdentityErrorMapper.ToError(result.Errors));
    }

    public async Task<Result<UserDto>> GetProfile(string userId)
    {
        if (await _userManager.FindByIdAsync(userId) is not { } user)
            return Result.Failure<UserDto>(UserErrors.UserNotFound);

        var roles = await _userManager.GetRolesAsync(user);

        var dto = user.Adapt<UserDto>();

        dto.Roles = roles.ToList();

        return Result.Success(dto);
    }
}
