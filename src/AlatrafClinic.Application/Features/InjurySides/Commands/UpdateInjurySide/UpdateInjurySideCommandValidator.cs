using FluentValidation;

namespace AlatrafClinic.Application.Features.InjurySides.Commands.UpdateInjurySide;

public class UpdateInjurySideCommandValidator : AbstractValidator<UpdateInjurySideCommand>
{
    public UpdateInjurySideCommandValidator()
    {
        RuleFor(x=> x.InjurySideId)
            .GreaterThan(0).WithMessage("Injury side ID must be greater than zero.");
        RuleFor(x=> x.Name)
            .NotEmpty().WithMessage("Injury side name is required.")
            .MaximumLength(100).WithMessage("Injury side name must not exceed 100 characters.");
    }
}