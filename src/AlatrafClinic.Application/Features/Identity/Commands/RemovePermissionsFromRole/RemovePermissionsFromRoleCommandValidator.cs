using FluentValidation;

namespace AlatrafClinic.Application.Features.Identity.Commands.RemovePermissionsFromRole;

public sealed class RemovePermissionsFromRoleCommandValidator
    : AbstractValidator<RemovePermissionsFromRoleCommand>
{
    public RemovePermissionsFromRoleCommandValidator()
    {
        RuleFor(x => x.RoleName)
            .NotEmpty()
            .WithMessage("Role name is required.")
            .MaximumLength(256);

        RuleForEach(x => x.PermissionNames)
            .NotEmpty()
            .WithMessage("Permission name is required.")
            .MaximumLength(200);
    }
}