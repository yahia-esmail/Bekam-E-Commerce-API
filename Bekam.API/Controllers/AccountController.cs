using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Bekam.API.Extensions;
using Bekam.Application.Abstraction.Consts;
using Bekam.Application.Abstraction.Contracts.Services;
using Bekam.Application.Abstraction.Contracts.Services.Identity;
using Bekam.Application.DTOs.Auth.change_password;

namespace Bekam.API.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AccountController(IAccountService accountService, ILoggedInUserService loggedInUserService) : ControllerBase
{
    private readonly IAccountService _accountService = accountService;
    private readonly ILoggedInUserService _loggedInUserService = loggedInUserService;

    [EnableRateLimiting(RateLimiters.UserLimiter)]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest req)
    {
        var result = await _accountService.ChangePasswordAsync(User, req);

        return result.ToApiResponse();
    }

    [EnableRateLimiting(RateLimiters.UserLimiter)]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfileAsync()
    {
        var result = await _accountService.GetProfile(_loggedInUserService.UserId!);

        return result.ToApiResponse();
    }


}
