using FluentValidation;

namespace AlatrafClinic.Application.Features.InjurySides.Commands.CreateInjurySide;

public class CreateInjurySideCommandValidator : AbstractValidator<CreateInjurySideCommand>
{
    public CreateInjurySideCommandValidator()
    {
        RuleFor(x=> x.Name)
            .NotEmpty().WithMessage("Injury side name is required.")
            .MaximumLength(100).WithMessage("Injury side name must not exceed 100 characters.");
    }
}