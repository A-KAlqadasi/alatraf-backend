using FluentValidation;

namespace AlatrafClinic.Application.Features.Identity.Commands.RemoveUserPermissionOverrides;

public sealed class RemoveUserPermissionOverridesCommandValidator
    : AbstractValidator<RemoveUserPermissionOverridesCommand>
{
    public RemoveUserPermissionOverridesCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.PermissionIds).NotNull();
    }
}
