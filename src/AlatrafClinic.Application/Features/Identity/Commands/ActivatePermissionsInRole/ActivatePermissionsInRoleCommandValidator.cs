using FluentValidation;

namespace AlatrafClinic.Application.Features.Identity.Commands.ActivatePermissionsInRole;

public sealed class ActivatePermissionsInRoleCommandValidator
    : AbstractValidator<ActivatePermissionsInRoleCommand>
{
    public ActivatePermissionsInRoleCommandValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty();

        RuleFor(x => x.PermissionIds)
            .NotNull();
    }
}
