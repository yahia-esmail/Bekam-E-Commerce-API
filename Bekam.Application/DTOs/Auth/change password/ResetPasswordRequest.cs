namespace Bekam.Application.DTOs.Auth.change_password;
public record ResetPasswordRequest(
    string Email,
    string Code,
    string NewPassword
);