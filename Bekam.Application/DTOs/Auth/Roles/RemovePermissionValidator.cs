using FluentValidation;
using Bekam.Application.DTOs.common;

namespace Bekam.Application.DTOs.Auth.Roles;
public class RemovePermissionValidator : AbstractValidator<RemovePermissionDto>
{
    public RemovePermissionValidator()
    {
        RuleFor(x => x.PermissionName).ValidPermission();
    }
}