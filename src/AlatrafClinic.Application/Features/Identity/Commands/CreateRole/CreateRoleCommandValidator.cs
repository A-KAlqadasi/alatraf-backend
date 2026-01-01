using FluentValidation;

namespace AlatrafClinic.Application.Features.Identity.Commands.CreateRole;

public sealed class CreateRoleCommandValidator
    : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();
    }
}
