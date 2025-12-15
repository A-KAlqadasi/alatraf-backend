using FluentValidation;

namespace AlatrafClinic.Application.Features.Identity.Commands.AddPermissionsToUser;

public sealed class AddPermissionsToUserCommandValidator
    : AbstractValidator<AddPermissionsToUserCommand>
{
    public AddPermissionsToUserCommandValidator()
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