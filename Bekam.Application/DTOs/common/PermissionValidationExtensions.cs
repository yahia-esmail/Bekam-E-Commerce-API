using FluentValidation;
using Bekam.Application.Abstraction.Consts;

namespace Bekam.Application.DTOs.common;
public static class PermissionValidationExtensions
{
    public static IRuleBuilderOptions<T, string> ValidPermission<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Permission is required.")
            .Must(p => Permissions.GetAllPermissions().Contains(p))
            .WithMessage("Invalid permission.");
    }
}
