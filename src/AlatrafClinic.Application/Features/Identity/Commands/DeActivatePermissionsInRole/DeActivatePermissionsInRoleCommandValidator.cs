using FluentValidation;

namespace AlatrafClinic.Application.Features.Identity.Commands.DeActivatePermissionsInRole;

public sealed class DeActivatePermissionsInRoleCommandValidator
    : AbstractValidator<DeActivatePermissionsInRoleCommand>
{
    public DeActivatePermissionsInRoleCommandValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty();

        RuleFor(x => x.PermissionIds)
            .NotNull();
    }
}
