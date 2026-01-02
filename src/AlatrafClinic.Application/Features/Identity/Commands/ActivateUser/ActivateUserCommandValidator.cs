using FluentValidation;

namespace AlatrafClinic.Application.Features.Identity.Commands.ActivateUser;

public sealed class ActivateUserCommandValidator
    : AbstractValidator<ActivateUserCommand>
{
    public ActivateUserCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}
