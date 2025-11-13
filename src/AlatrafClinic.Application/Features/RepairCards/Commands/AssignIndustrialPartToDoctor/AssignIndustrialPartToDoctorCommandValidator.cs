using FluentValidation;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.AssignIndustrialPartToDoctor;

public class AssignIndustrialPartToDoctorCommandValidator : AbstractValidator<AssignIndustrialPartToDoctorCommand>
{
    public AssignIndustrialPartToDoctorCommandValidator()
    {
        RuleFor(x => x.RepairCardId)
            .GreaterThan(0).WithMessage("Repair Card Id must be greater than zero.");

        RuleForEach(x => x.DoctorIndustrialParts)
            .SetValidator(new DoctorIndustrialPartCommandValidator());
    }
}