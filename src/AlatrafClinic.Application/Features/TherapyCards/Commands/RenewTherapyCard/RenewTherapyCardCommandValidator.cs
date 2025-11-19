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

        RuleForEach(x => x.Programs)
            .SetValidator(new RenewTherapyCardMedicalProgramCommandValidator());
    }
}