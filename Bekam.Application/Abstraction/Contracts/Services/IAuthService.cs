using Bekam.Application.Abstraction.Results;
using Bekam.Application.DTOs.Auth;
using Bekam.Application.DTOs.Auth.change_password;
using Bekam.Application.DTOs.Auth.ConfirmEmail;

namespace Bekam.Application.Abstraction.Contracts.Services;
public interface IAuthService
{
    Task<Result<AuthResponse>> GetTokenAsync(LoginRequest request, CancellationToken cancellationToken);
    Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> RefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
    Task<Result> RevokeRefreshTokenAsync(string userId, string refreshToken, CancellationToken cancellationToken = default);
    Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request);
    Task<Result> ResendConfirmationEmailAsync(ResendConfirmationEmailRequest request);
    Task<Result> SendResetPasswordCodeAsync(string email);
    Task<Result> ResetPasswordAsync(ResetPasswordRequest request);

}
