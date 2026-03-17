using FluentValidation;

namespace Bekam.Application.DTOs.Auth;
public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop) // Stop on first failure, use when errors depend on each other in order.
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email.");


        RuleFor(x => x.Password) 
            .NotEmpty().WithMessage("Password is required.");
    }
}