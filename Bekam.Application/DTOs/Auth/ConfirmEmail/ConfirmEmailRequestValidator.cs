using FluentValidation;

namespace Bekam.Application.DTOs.Auth.ConfirmEmail;
internal class ConfirmEmailRequestValidator : AbstractValidator<ConfirmEmailRequest>
{
    public ConfirmEmailRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("User Id is required.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required.");
    }
}