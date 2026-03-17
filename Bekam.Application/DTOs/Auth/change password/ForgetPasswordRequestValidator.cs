using FluentValidation;

namespace Bekam.Application.DTOs.Auth.change_password;
public class ForgetPasswordRequestValidator : AbstractValidator<ForgetPasswordRequest>
{
    public ForgetPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop) // Stop on first failure, use when errors depend on each other in order.
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email.");
    }
}