using FluentValidation;
using Bekam.Application.DTOs.common;

namespace Bekam.Application.DTOs.Auth.Roles;
public class CreateRoleValidator : AbstractValidator<CreateRoleDto>
{
    public CreateRoleValidator()
    {
        RuleFor(x => x.Name)
            .ValidName(50);
    }
}