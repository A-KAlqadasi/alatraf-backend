using FluentValidation;

namespace AlatrafClinic.Application.Features.Identity.Commands.AssignUserToRole;

public sealed class AssignRoleToUserCommandValidator
    : AbstractValidator<AssignRoleToUserCommand>
{
    public AssignRoleToUserCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.RoleId).NotEmpty();
    }
}
