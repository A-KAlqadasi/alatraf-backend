using FluentValidation;

namespace AlatrafClinic.Application.Features.TherapyCards.Commands.GenerateSessions;

public class GenerateSessionsCommandValidator : AbstractValidator<GenerateSessionsCommand>
{
    public GenerateSessionsCommandValidator()
    {
        RuleFor(x => x.TherapyCardId)
            .GreaterThan(0).WithMessage("TherapyCardId must be greater than zero.");
    }
}