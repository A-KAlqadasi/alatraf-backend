using FluentValidation;

namespace AlatrafClinic.Application.Features.Identity.Commands.UpsertUserRoles;

public sealed class UpsertUserRolesCommandValidator
    : AbstractValidator<UpsertUserRolesCommand>
{
    public UpsertUserRolesCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.RoleIds).NotEmpty();
    }
}

