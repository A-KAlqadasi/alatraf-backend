using FluentValidation;

namespace AlatrafClinic.Application.Features.Identity.Commands.RemovePermissionFromUser;

public sealed class RemovePermissionFromUserCommandValidator
    : AbstractValidator<RemovePermissionFromUserCommand>
{
    public RemovePermissionFromUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User id is required.")
            .MaximumLength(450);

        RuleFor(x => x.PermissionName)
            .NotEmpty()
            .WithMessage("Permission name is required.")
            .MaximumLength(200);
    }
}