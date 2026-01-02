using FluentValidation;

namespace AlatrafClinic.Application.Features.Identity.Commands.AssignRolesToUser;

public sealed class AssignRolesToUserCommandValidator
    : AbstractValidator<AssignRolesToUserCommand>
{
    public AssignRolesToUserCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.RoleIds).NotEmpty();
    }
}
