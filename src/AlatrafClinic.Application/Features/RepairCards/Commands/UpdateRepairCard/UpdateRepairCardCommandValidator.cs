using FluentValidation;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.UpdateRepairCard;

public class UpdateRepairCardCommandValidator : AbstractValidator<UpdateRepairCardCommand>
{
    public UpdateRepairCardCommandValidator()
    {
        RuleFor(x => x.RepairCardId)
            .GreaterThan(0);
        RuleFor(x => x.TicketId)
            .GreaterThan(0).WithMessage("TicketId must be greater than 0.");
        RuleFor(x => x.DiagnosisText)
            .NotEmpty().WithMessage("DiagnosisText must not be empty.")
            .MaximumLength(1000).WithMessage("DiagnosisText must not exceed 1000 characters.");
        RuleForEach(x => x.InjuryReasons)
            .GreaterThan(0).WithMessage("InjuryReasons must contain valid reason IDs.");
        RuleForEach(x => x.InjurySides)
            .GreaterThan(0).WithMessage("InjurySides must contain valid side IDs.");
        RuleForEach(x => x.InjuryTypes)
            .GreaterThan(0).WithMessage("InjuryTypes must contain valid type IDs.");
        RuleForEach(x => x.IndustrialParts)
            .SetValidator(new UpdateRepairCardIndustrialPartCommandValidator());
        RuleFor(x=> x.IndustrialParts)
            .NotEmpty().WithMessage("At least one industrial part must be provided.");
    }
}