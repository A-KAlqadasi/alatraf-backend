using FluentValidation;

namespace AlatrafClinic.Application.Features.InjurySides.Commands.DeleteInjurySide;
public class DeleteInjurySideCommandValidator : AbstractValidator<DeleteInjurySideCommand>
{
    public DeleteInjurySideCommandValidator()
    {
        RuleFor(x=> x.InjurySideId)
            .GreaterThan(0).WithMessage("Injury side ID must be greater than zero.");
    }
}