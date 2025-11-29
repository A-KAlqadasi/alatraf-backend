using FluentValidation;

namespace AlatrafClinic.Application.Features.Identity.Commands.AddPermissionToUser;

public sealed class AddPermissionToUserCommandValidator
    : AbstractValidator<AddPermissionToUserCommand>
{
    public AddPermissionToUserCommandValidator()
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