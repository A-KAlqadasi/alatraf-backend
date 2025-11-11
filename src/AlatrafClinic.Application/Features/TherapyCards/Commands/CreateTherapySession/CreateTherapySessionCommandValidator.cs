using FluentValidation;

namespace AlatrafClinic.Application.Features.TherapyCards.Commands.CreateTherapySession;

public class CreateTherapySessionCommandValidator : AbstractValidator<CreateTherapySessionCommand>
{
    public CreateTherapySessionCommandValidator()
    {
        RuleFor(x => x.TherapyCardId)
            .GreaterThan(0).WithMessage("Therapy card Id is invalid");
        RuleFor(x => x.SessionProgramsData)
            .NotEmpty()
            .WithMessage("At least one session program data must be specified.");
    }
}