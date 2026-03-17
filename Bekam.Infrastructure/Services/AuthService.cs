using FluentValidation;
using Hangfire;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;
using Bekam.Application.Abstraction.Consts;
using Bekam.Application.Abstraction.Contracts.Security;
using Bekam.Application.Abstraction.Contracts.Services;
using Bekam.Application.Abstraction.Errors;
using Bekam.Application.Abstraction.Results;
using Bekam.Application.DTOs.Auth;
using Bekam.Application.DTOs.Auth.change_password;
using Bekam.Application.DTOs.Auth.ConfirmEmail;
using Bekam.Domain.Entities.Identity;
using Bekam.Infrastructure.Helpers;
using Bekam.Infrastructure.Persistence._Identity;
namespace Bekam.Infrastructure.Services;
internal class AuthService(UserManager<ApplicationUser> userManager,
                           SignInManager<ApplicationUser> _signInManager,
                           IdentityDbContext dbContext,
                           IJwtProvider jwtProvider,
                           IMapper mapper,
                           IConfiguration configuration,
                           IEmailService emailService,
                           IBackgroundJobService background,
                           IHttpContextAccessor httpContextAccessor) : IAuthService
{

    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IdentityDbContext _dbContext = dbContext;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly IMapper _mapper = mapper;
    private readonly IConfiguration _config = configuration;
    private readonly IEmailService _emailService = emailService;
    private readonly IBackgroundJobService _background = background;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly int _refreshTokenExpiryDays = 14;

    public async Task<Result<AuthResponse>> GetTokenAsync(LoginRequest request, CancellationToken ct)
    {
        // check if user exists and password is correct
        if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

        if (!await _userManager.CheckPasswordAsync(user, request.Password))
        {
            await _userManager.AccessFailedAsync(user);

            if (await _userManager.IsLockedOutAsync(user))
                return Result.Failure<AuthResponse>(UserErrors.LockedUser);

            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
        }

        if (!user.IsActive)
            return Result.Failure<AuthResponse>(UserErrors.InactiveUser);

        // here password is correct, but will checked again with other policies
        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, true);

        if (result.Succeeded)
        {
            var (userRoles, userPermissions) = await GetUserRolesAndPermissions(user, ct);

            var (token, expiresIn) = _jwtProvider.GenerateToken(user, userRoles, userPermissions);

            //refresh token
            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                ExpiresOn = refreshTokenExpiration
            });

            await _userManager.UpdateAsync(user);

            var response = new AuthResponse(user.Id, user.Email!, user.FirstName, user.LastName, token, expiresIn, refreshToken, refreshTokenExpiration);

            return Result.Success(response);
        }


        var error = result.IsNotAllowed ? UserErrors.EmailNotConfirmed : result.IsLockedOut ? UserErrors.LockedUser
                    : UserErrors.InvalidCredentials;

        return Result.Failure<AuthResponse>(error);
    }

    public async Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        if (await _userManager.FindByEmailAsync(request.Email) is not null)
            return Result.Failure(UserErrors.DuplicatedEmail);

        var user = request.Adapt<ApplicationUser>();

        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, DefaultRoles.Member);

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

#pragma warning disable CS4014
            _background.Enqueue(() => SendConfirmationEmailJob(user.Id, code));
#pragma warning restore CS4014

            return Result.Success();
        }

        return Result.Failure(IdentityErrorMapper.ToError(result.Errors));
    }

    public async Task<Result<AuthResponse>> RefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token, false);

        if (userId is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidAccessToken);

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidAccessToken);

        if (!user.IsActive)
            return Result.Failure<AuthResponse>(UserErrors.InactiveUser);

        if (user.LockoutEnd > DateTime.UtcNow)
            return Result.Failure<AuthResponse>(UserErrors.LockedUser);

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

        if (userRefreshToken is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidRefreshToken);

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        var (userRoles, userPermissions) = await GetUserRolesAndPermissions(user, cancellationToken);

        var (newToken, expiresIn) = _jwtProvider.GenerateToken(user, userRoles, userPermissions);
        var newRefreshToken = GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = newRefreshToken,
            ExpiresOn = refreshTokenExpiration
        });

        await _userManager.UpdateAsync(user);

        var response = new AuthResponse(user.Id, user.Email!, user.FirstName, user.LastName, newToken, expiresIn, newRefreshToken, refreshTokenExpiration);

        return Result.Success(response);
    }

    public async Task<Result> RevokeRefreshTokenAsync(string userId, string refreshToken, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return Result.Failure(UserErrors.InvalidAccessToken);

        var userRefreshToken = user.RefreshTokens
            .SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

        if (userRefreshToken is null)
            return Result.Failure(UserErrors.InvalidRefreshToken);

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);

        return Result.Success();
    }

    public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
    {
        if (await _userManager.FindByIdAsync(request.Id) is not { } user)
            return Result.Failure<string>(UserErrors.UserNotFound);

        if (user.EmailConfirmed)
            return Result.Failure<string>(UserErrors.EmailAlreadyConfirmed);

        var code = request.Code;

        try
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        }
        catch (FormatException)
        {
            return Result.Failure<string>(UserErrors.InvalidCode);
        }

        var result = await _userManager.ConfirmEmailAsync(user, code);

        if (result.Succeeded)
            return Result.Success();
        
        return Result.Failure(IdentityErrorMapper.ToError(result.Errors));
    }

    public async Task<Result> ResendConfirmationEmailAsync(ResendConfirmationEmailRequest request)
    {
        if (await _userManager.FindByIdAsync(request.Id) is not { } user)
            return Result.Failure<string>(UserErrors.UserNotFound);

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailAlreadyConfirmed);

        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

#pragma warning disable CS4014
        _background.Enqueue(() => SendConfirmationEmailJob(user.Id, code));
#pragma warning restore CS4014

        return Result.Success();
    }

    public async Task<Result> SendResetPasswordCodeAsync(string email)
    {
        if (await _userManager.FindByEmailAsync(email) is not { } user)
            return Result.Failure(UserErrors.UserNotFound);
        //return Result.Success(); // to more security.

        if (!user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailNotConfirmed);

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        await SendResetPasswordEmail(user, code);

        return Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
    {
        if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
            return Result.Failure(UserErrors.UserNotFound);

        if (!user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailNotConfirmed);

        IdentityResult result;

        try
        {
            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
            result = await _userManager.ResetPasswordAsync(user, code, request.NewPassword);
        }
        catch (FormatException)
        {
            result = IdentityResult.Failed(_userManager.ErrorDescriber.InvalidToken());
        }

        if (!result.Succeeded)
            return Result.Failure(IdentityErrorMapper.ToError(result.Errors));

        // revoke refresh tokens
        user.RefreshTokens.Clear();
        await _userManager.UpdateAsync(user);

        return Result.Success();
    }

    /* Helper Methods */
    private async Task SendResetPasswordEmail(ApplicationUser user, string code)
    {
        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

        var emailBody = HtmlBodyBuilder.GenerateResetPasswordEmailPage(user, code, origin);

        BackgroundJob.Enqueue(() => _emailService.SendEmailAsync(user.Email!, "✅ Bekam: Reset Password", emailBody));

        await Task.CompletedTask;
    }
    public async Task SendConfirmationEmailJob(string userId, string code)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null || user.EmailConfirmed)
            return;

        var body = HtmlBodyBuilder.GenerateConfirmationEmailPage(user, code, _config);

        await _emailService.SendEmailAsync(user.Email!, "✅ Bekam: Email Confirmation", body);
    }
    private async Task<(IEnumerable<string> roles, IEnumerable<string> permissions)> GetUserRolesAndPermissions(ApplicationUser user, CancellationToken cancellationToken)
    {
        var userRoles = await _userManager.GetRolesAsync(user);

        var userPermissions = await (from r in _dbContext.Roles
                                     join p in _dbContext.RoleClaims
                                     on r.Id equals p.RoleId
                                     where userRoles.Contains(r.Name!)
                                     select p.ClaimValue!)
                                     .Distinct()
                                     .ToListAsync(cancellationToken);

        return (userRoles, userPermissions);
    }

    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }


}
