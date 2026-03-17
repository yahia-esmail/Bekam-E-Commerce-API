namespace Bekam.Application.DTOs.Auth.ConfirmEmail;
public record ConfirmEmailRequest(
    string Id,
    string Code
);