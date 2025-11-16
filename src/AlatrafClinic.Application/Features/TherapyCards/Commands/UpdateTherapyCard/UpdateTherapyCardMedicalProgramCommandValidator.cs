using FluentValidation;

namespace AlatrafClinic.Application.Features.TherapyCards.Commands.UpdateTherapyCard;

public class UpdateTherapyCardMedicalProgramCommandValidator : AbstractValidator<UpdateTherapyCardMedicalProgramCommand>
{
    public UpdateTherapyCardMedicalProgramCommandValidator()
    {
        RuleFor(x => x.MedicalProgramId)
            .GreaterThan(0).WithMessage("MedicalProgramId must be greater than 0.");

        RuleFor(x => x.Duration)
            .GreaterThan(0).WithMessage("Duration must be greater than 0.");
    }
}