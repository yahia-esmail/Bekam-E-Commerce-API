using FluentValidation;

namespace Bekam.Application.DTOs.Auth;
internal class RevokeRefreshTokenValidator : AbstractValidator<RevokeRefreshToken>
{
    public RevokeRefreshTokenValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh Token is required.");
    }
}