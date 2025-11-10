using FluentValidation;

namespace AlatrafClinic.Application.Features.MedicalPrograms.Commands.DeleteMedicalProgram;

public class DeleteMedicalProgramCommandValidator : AbstractValidator<DeleteMedicalProgramCommand>
{
    public DeleteMedicalProgramCommandValidator()
    {
        RuleFor(x => x.MedicalProgramId)
            .GreaterThan(0).WithMessage("Medical program ID must be greater than zero.");
    }
}