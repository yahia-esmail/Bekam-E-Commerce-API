using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Bekam.API.Extensions;
using Bekam.Application.Abstraction.Consts;
using Bekam.Application.Abstraction.Contracts.Services;
using Bekam.Application.Abstraction.Results;
using Bekam.Application.DTOs.Auth;
using Bekam.Application.DTOs.Auth.change_password;
using Bekam.Application.DTOs.Auth.ConfirmEmail;

namespace Bekam.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [Authorize]
    [EnableRateLimiting(RateLimiters.UserLimiter)]
    [HttpGet("me")]
    
    public IActionResult GetCurrentUserInfo()
    {
        foreach (var claim in User.Claims)
        {
            Console.WriteLine($"{claim.Type} : {claim.Value}");
        }

        var userInfo = new UserInfoResponse
        {
            id = User.FindFirstValue(ClaimTypes.NameIdentifier),
            email = User.FindFirstValue(ClaimTypes.Email),
            displayedName = User.FindFirstValue(JwtRegisteredClaimNames.Name),
            roles = User.FindAll(ClaimTypes.Role)
                        .Select(r => r.Value)
                        .ToList()
        };

        return Result.Success(userInfo).ToApiResponse();
    }

    [EnableRateLimiting(RateLimiters.StrictIpLimiter)]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest, CancellationToken cancellationToken)
    {
        var result = await _authService.GetTokenAsync(loginRequest, cancellationToken);

        return result.ToApiResponse();
    }

    [EnableRateLimiting(RateLimiters.StrictIpLimiter)]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(request, cancellationToken);

        return result.IsSuccess ? Ok(new
        {
            Message = "Registration successful. Please check your email to confirm your account."
        }) : result.ToApiResponse();
    }

    
    [EnableRateLimiting(RateLimiters.StrictIpLimiter)]
    [HttpPost("refresh-token")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

        return result.ToApiResponse();
    }

    
    [Authorize]
    [EnableRateLimiting(RateLimiters.UserLimiter)]
    [HttpPost("logout")]
    public async Task<IActionResult> RevokeRefreshToken([FromBody] RevokeRefreshToken req, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _authService.RevokeRefreshTokenAsync(userId!, req.RefreshToken, cancellationToken);

        return result.ToApiResponse();
    }

    
    [HttpGet("resend-confirmation-email")]
    public async Task<IActionResult> ResendConfirmationEmail([FromQuery] ResendConfirmationEmailRequest request)
    {
        var result = await _authService.ResendConfirmationEmailAsync(request);

        return result.ToApiResponse();
    }

    
    [EnableRateLimiting(RateLimiters.StrictIpLimiter)]
    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.ConfirmEmailAsync(request);

        return result.ToApiResponse();
    }

    
    [EnableRateLimiting(RateLimiters.StrictIpLimiter)]
    [HttpPost("forget-password")]
    public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequest request)
    {
        var result = await _authService.SendResetPasswordCodeAsync(request.Email);

        return result.ToApiResponse();
    }

    
    [EnableRateLimiting(RateLimiters.StrictIpLimiter)]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var result = await _authService.ResetPasswordAsync(request);

        return result.ToApiResponse();
    }

}
