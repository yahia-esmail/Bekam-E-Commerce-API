using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Bekam.Application.DTOs.categories;
public class CreateCategoryValidator : AbstractValidator<CreateCategoryDto>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(100).WithMessage("Category name must not exceed 100 characters.");

        RuleFor(x => x.ParentCategoryId)
            .GreaterThan(0)
            .When(x => x.ParentCategoryId.HasValue)
            .WithMessage("Invalid parent category id.");

        RuleFor(x => x.Picture)
            .Must(BeValidImage)
            .When(x => x.Picture != null)
            .WithMessage("Only image files (jpg, jpeg, png, webp) are allowed.")
            .Must(BeValidSize)
            .When(x => x.Picture != null)
            .WithMessage("Image size must not exceed 2MB.");
    }

    private bool BeValidImage(IFormFile file)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLower();
        return allowedExtensions.Contains(extension) &&
               file.ContentType.StartsWith("image/");
    }

    private bool BeValidSize(IFormFile file)
    {
        const int maxSize = 2 * 1024 * 1024;
        return file.Length <= maxSize;
    }
}