using FluentValidation;
using Bekam.Application.DTOs.common;

namespace Bekam.Application.DTOs.Auth.Roles;
public class UpdateRoleValidator : AbstractValidator<UpdateRoleDto>
{
    public UpdateRoleValidator()
    {
        RuleFor(x => x.Name)
            .ValidName(50);
    }
}