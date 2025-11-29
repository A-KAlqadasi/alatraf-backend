using FluentValidation;

namespace AlatrafClinic.Application.Features.Identity.Commands.AddPermissionToRole;

public sealed class AddPermissionToRoleCommandValidator
    : AbstractValidator<AddPermissionToRoleCommand>
{
    public AddPermissionToRoleCommandValidator()
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