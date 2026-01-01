using FluentValidation;

namespace AlatrafClinic.Application.Features.Identity.Commands.AssignPermissionsToRole;

public sealed class AssignPermissionsToRoleCommandValidator
    : AbstractValidator<AssignPermissionsToRoleCommand>
{
    public AssignPermissionsToRoleCommandValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty();

        RuleFor(x => x.PermissionIds)
            .NotNull();
    }
}
