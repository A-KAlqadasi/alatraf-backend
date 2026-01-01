using System.Data;

using FluentValidation;

namespace AlatrafClinic.Application.Features.TherapyCards.Commands.CreateTherapyCard;

public class CreateTherapyCardCommandValidator : AbstractValidator<CreateTherapyCardCommand>
{
    public CreateTherapyCardCommandValidator()
    {
        RuleFor(x => x.TicketId)
            .GreaterThan(0).WithMessage("TicketId must be greater than 0.");
        
        RuleFor(x => x.DiagnosisText)
            .NotEmpty().WithMessage("DiagnosisText must not be empty.")
            .MaximumLength(2000).WithMessage("DiagnosisText must not exceed 2000 characters.");
        RuleForEach(x => x.InjuryReasons)
            .GreaterThan(0).WithMessage("InjuryReasons must contain valid reason IDs.");
        RuleForEach(x => x.InjurySides)
            .GreaterThan(0).WithMessage("InjurySides must contain valid side IDs.");
        RuleForEach(x => x.InjuryTypes)
            .GreaterThan(0).WithMessage("InjuryTypes must contain valid type IDs.");
        RuleFor(x => x.TherapyCardType)
            .IsInEnum().WithMessage("Type must be a valid Therapy Card Type.");
        RuleFor(x => x.ProgramStartDate)
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today)).WithMessage("Program Start Date must not be in the past.");

        RuleFor(x=> x.NumberOfSessions)
            .GreaterThan(0).WithMessage("Number of sessions must be greater than zero");
        RuleForEach(x => x.Programs)
            .SetValidator(new CreateTherapyCardMedicalProgramCommandValidator());
    }
}