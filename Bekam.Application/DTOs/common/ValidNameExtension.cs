using FluentValidation;

namespace Bekam.Application.DTOs.common;
public static class ValidNameExtension
{
    public static IRuleBuilderOptions<T, string> ValidName<T>(
    this IRuleBuilder<T, string> ruleBuilder,
    int maxLength = 100)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(maxLength)
                .WithMessage($"Name must not exceed {maxLength} characters.")
            .Must(name => !string.IsNullOrWhiteSpace(name?.Trim()))
                .WithMessage("Name cannot be empty or whitespace.");
    }
}