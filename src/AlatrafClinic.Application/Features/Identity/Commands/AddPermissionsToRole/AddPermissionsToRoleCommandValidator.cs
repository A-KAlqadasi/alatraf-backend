using FluentValidation;

namespace AlatrafClinic.Application.Features.Identity.Commands.AddPermissionsToRole;

public sealed class AddPermissionsToRoleCommandValidator
    : AbstractValidator<AddPermissionsToRoleCommand>
{
    public AddPermissionsToRoleCommandValidator()
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