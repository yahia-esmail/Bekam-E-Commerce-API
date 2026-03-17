using FluentValidation;
using Bekam.Application.Abstraction.Consts;

namespace Bekam.Application.DTOs.Auth.change_password;
public class ResetPasswordRequestValidators : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidators()
    {
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop) // Stop on first failure, use when errors depend on each other in order.
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required.");

        RuleFor(x => x.NewPassword)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Password is required.")
            .Matches(RegexPatterns.Password)
            .WithMessage("Password must be at least 8 characters and include uppercase, lowercase, and a special character.");
    }
}