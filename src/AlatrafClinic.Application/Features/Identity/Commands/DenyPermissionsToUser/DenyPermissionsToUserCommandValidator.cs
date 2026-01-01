using FluentValidation;

namespace AlatrafClinic.Application.Features.Identity.Commands.DenyPermissionsToUser;

public sealed class DenyPermissionsToUserCommandValidator
    : AbstractValidator<DenyPermissionsToUserCommand>
{
    public DenyPermissionsToUserCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.PermissionIds).NotNull();
    }
}
