using FluentValidation;

namespace AlatrafClinic.Application.Features.Diagnosises.Commands.DeleteDiagnosis;

public class DeleteDiagnosisCommandValidator : AbstractValidator<DeleteDiagnosisCommand>
{
    public DeleteDiagnosisCommandValidator()
    {
        RuleFor(x => x.diagnosisId)
            .GreaterThan(0).WithMessage("Diagnosis Id must be greater than zero.");
    }
}