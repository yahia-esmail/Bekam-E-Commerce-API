using FluentValidation;
using Bekam.Application.DTOs.common;

namespace Bekam.Application.DTOs.Auth.Roles;

public class AddPermissionValidator : AbstractValidator<AddPermissionDto>
{
    public AddPermissionValidator()
    {
        RuleFor(x => x.PermissionName).ValidPermission();
    }
}