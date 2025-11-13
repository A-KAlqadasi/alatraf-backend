using FluentValidation;

namespace AlatrafClinic.Application.Features.TherapyCards.Commands.RenewTherapyCard;

public class RenewTherapyCardCommandValidator : AbstractValidator<RenewTherapyCardCommand>
{
    public RenewTherapyCardCommandValidator()
    {
        RuleFor(x => x.TicketId)
            .GreaterThan(0);
        RuleFor(x => x.TherapyCardId)
            .GreaterThan(0);

        RuleFor(x => x.ProgramStartDate)
            .LessThan(x => x.ProgramEndDate)
            .WithMessage("Program start date must be earlier than program end date.");

        RuleFor(x => x.Programs)
            .NotEmpty()
            .WithMessage("At least one medical program must be specified.");

        RuleForEach(x => x.Programs).ChildRules(program =>
        {
            program.RuleFor(p => p.MedicalProgramId)
                .GreaterThan(0)
                .WithMessage("Medical program ID must be greater than zero.");

            program.RuleFor(p => p.Duration)
                .GreaterThan(0)
                .WithMessage("Duration must be greater than zero.");
        });
    }
}