using FluentValidation;

namespace AlatrafClinic.Application.Features.TherapyCards.Commands.GenerateSessions;

public class TakeSessionCommandValidator : AbstractValidator<TakeSessionCommand>
{
    public TakeSessionCommandValidator()
    {
        RuleFor(x => x.TherapyCardId)
            .GreaterThan(0).WithMessage("TherapyCardId must be greater than 0.");

        RuleFor(x => x.SessionId)
            .GreaterThan(0).WithMessage("SessionId must be greater than 0.");

        RuleFor(x => x.SessionProgramsData)
            .NotNull().WithMessage("SessionProgramsData cannot be null.")
            .Must(x => x.Count > 0).WithMessage("SessionProgramsData must contain at least one item.");
    }
}