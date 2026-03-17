using FluentValidation;

namespace Bekam.Application.DTOs.Product;
public class UpdateProductRequestValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product Name is required.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Product Description is required.");

        RuleFor(x => x.Picture)
            .Must(file => file.Length <= 2 * 1024 * 1024)
            .WithMessage("Image must not exceed 2MB")
            .Must(file =>
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var extension = Path.GetExtension(file.FileName).ToLower();
                return allowedExtensions.Contains(extension);
            })
            .WithMessage("Only image files are allowed")
            .When(x => x.Picture is not null);

        RuleFor(x => x.Price)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Product Price is required.")
            .GreaterThanOrEqualTo(0).WithMessage("Price must be a non-negative value.");
    }
}