using FluentValidation;

namespace AlatrafClinic.Application.Features.Identity.Commands.GrantPermissionsToUser;

public sealed class GrantPermissionsToUserCommandValidator
    : AbstractValidator<GrantPermissionsToUserCommand>
{
    public GrantPermissionsToUserCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.PermissionIds).NotNull();
    }
}
