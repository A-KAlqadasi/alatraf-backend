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

        RuleFor(x=> x.NumberOfSessions)
            .GreaterThan(0).WithMessage("Number of sessions must be geater than zero");
            
        RuleFor(x => x.Programs)
            .NotEmpty()
            .WithMessage("At least one medical program must be specified.");
        RuleFor(x => x.ProgramStartDate)
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today)).WithMessage("Program Start Date must not be in the past.");

        RuleForEach(x => x.Programs)
            .SetValidator(new RenewTherapyCardMedicalProgramCommandValidator());
    }
}