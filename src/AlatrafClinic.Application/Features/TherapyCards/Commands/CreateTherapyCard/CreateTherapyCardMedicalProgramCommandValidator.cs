using FluentValidation;

namespace AlatrafClinic.Application.Features.TherapyCards.Commands.CreateTherapyCard;

public class CreateTherapyCardMedicalProgramCommandValidator : AbstractValidator<CreateTherapyCardMedicalProgramCommand>
{
    public CreateTherapyCardMedicalProgramCommandValidator()
    {
        RuleFor(x => x.MedicalProgramId)
            .GreaterThan(0).WithMessage("MedicalProgramId must be greater than 0.");

        RuleFor(x => x.Duration)
            .GreaterThan(0).WithMessage("Duration must be greater than 0.");
    }
}