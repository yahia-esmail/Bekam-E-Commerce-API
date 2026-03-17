namespace Bekam.Application.DTOs.Auth.change_password;
public class ChangePasswordRequest
{
    public required string CurrentPassword { get; set; }
    public required string NewPassword { get; set; }
}
