using FluentValidation;

namespace AlatrafClinic.Application.Features.Identity.Commands.RemovePermissionsFromUser;

public sealed class RemovePermissionsFromUserCommandValidator
    : AbstractValidator<RemovePermissionsFromUserCommand>
{
    public RemovePermissionsFromUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User id is required.")
            .MaximumLength(450);

        RuleForEach(x => x.PermissionNames)
            .NotEmpty()
            .WithMessage("Permission name is required.")
            .MaximumLength(200);
    }
}