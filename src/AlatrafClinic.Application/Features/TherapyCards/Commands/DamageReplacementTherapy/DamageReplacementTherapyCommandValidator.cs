using FluentValidation;

namespace AlatrafClinic.Application.Features.TherapyCards.Commands.DamageReplacementTherapy;

public class DamageReplacementTherapyCommandValidator : AbstractValidator<DamageReplacementTherapyCommand>
{
    public DamageReplacementTherapyCommandValidator()
    {
        RuleFor(x=> x.TicketId)
            .GreaterThan(0).WithMessage("TicketId must be greater than 0.");
        RuleFor(x => x.TherapyCardId)
            .GreaterThan(0).WithMessage("TherapyCardId must be greater than 0.");
    }
}