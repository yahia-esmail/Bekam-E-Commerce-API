using FluentValidation;

namespace Bekam.Application.DTOs.Brands;
public class UpdateBrandValidator : AbstractValidator<UpdateBrandDto>
{
    public UpdateBrandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Brand name is required.")
            .MaximumLength(100).WithMessage("Brand name must not exceed 100 characters.")
            .Must(BeValidName).WithMessage("Brand name must contain valid characters only.");
    }

    private bool BeValidName(string name)
    {
        return !string.IsNullOrWhiteSpace(name?.Trim());
    }
}