using FluentValidation;

namespace AlatrafClinic.Application.Features.MedicalPrograms.Commands.UpdateMedicalProgram;


public class UpdateMedicalProgramCommandValidator : AbstractValidator<UpdateMedicalProgramCommand>
{
    public UpdateMedicalProgramCommandValidator()
    {
        RuleFor(x => x.MedicalProgramId)
            .GreaterThan(0).WithMessage("Medical program ID must be greater than zero.");
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Medical program name is required.")
            .MaximumLength(200).WithMessage("Medical program name must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
    }
}