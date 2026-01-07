using FluentValidation;

namespace AlatrafClinic.Application.Features.Identity.Commands.DeleteRole;

public sealed class DeleteRoleCommandValidator
    : AbstractValidator<DeleteRoleCommand>
{
    public DeleteRoleCommandValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty();
    }
}
