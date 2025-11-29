using FluentValidation;

namespace AlatrafClinic.Application.Features.Identity.Commands.RemovePermissionFromRole;

public sealed class RemovePermissionFromRoleCommandValidator
    : AbstractValidator<RemovePermissionFromRoleCommand>
{
    public RemovePermissionFromRoleCommandValidator()
    {
        RuleFor(x => x.RoleName)
            .NotEmpty()
            .WithMessage("Role name is required.")
            .MaximumLength(256);

        RuleFor(x => x.PermissionName)
            .NotEmpty()
            .WithMessage("Permission name is required.")
            .MaximumLength(200);
    }
}