using FluentValidation;

namespace AlatrafClinic.Application.Features.Identity.Commands.RemovePermissionsFromRole;

public sealed class RemovePermissionsFromRoleCommandValidator
    : AbstractValidator<RemovePermissionsFromRoleCommand>
{
    public RemovePermissionsFromRoleCommandValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty();

        RuleFor(x => x.PermissionIds)
            .NotNull();
    }
}
