using FluentValidation;

namespace AlatrafClinic.Application.Features.Identity.Commands.UpsertUserPermissions;

public sealed class UpsertUserPermissionsCommandValidator
    : AbstractValidator<UpsertUserPermissionsCommand>
{
    public UpsertUserPermissionsCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.PermissionIds).NotNull();
    }
}
