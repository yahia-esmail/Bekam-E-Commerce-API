using FluentValidation;

namespace Bekam.Application.DTOs.Auth.Roles;
public class AssignUserRoleValidator : AbstractValidator<AssignUserRoleDto>
{
    public AssignUserRoleValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("Role id is required.");
    }
}