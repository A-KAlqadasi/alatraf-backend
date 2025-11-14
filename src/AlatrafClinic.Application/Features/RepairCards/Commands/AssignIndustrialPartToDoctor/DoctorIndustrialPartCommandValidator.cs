using FluentValidation;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.AssignIndustrialPartToDoctor;

public class DoctorIndustrialPartCommandValidator : AbstractValidator<DoctorIndustrialPartCommand>
{
    public DoctorIndustrialPartCommandValidator()
    {
        RuleFor(x => x.DiagnosisIndustrialPartId)
            .GreaterThan(0).WithMessage("Diagnosis Industrial Part Id must be greater than zero.");

        RuleFor(x => x.DoctorId)
            .GreaterThan(0).WithMessage("Doctor Id must be greater than zero.");
        RuleFor(x => x.SectionId)
            .GreaterThan(0).WithMessage("Section Id must be greater than zero.");
    }
}