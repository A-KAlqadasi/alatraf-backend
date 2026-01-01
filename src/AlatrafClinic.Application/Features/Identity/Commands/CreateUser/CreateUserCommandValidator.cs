using FluentValidation;

namespace AlatrafClinic.Application.Features.Identity.Commands.CreateUser;

public sealed class CreateUserCommandValidator
    : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.PersonId).GreaterThan(0);
        RuleFor(x => x.UserName).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}
