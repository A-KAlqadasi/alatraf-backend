using FluentValidation;

namespace AlatrafClinic.Application.Features.Diagnosises.Queries.GetPatientDiagnosesType;

public class GetPatientDiagnosesTypeQueryValidator : AbstractValidator<GetPatientDiagnosesTypeQuery>
{
    public GetPatientDiagnosesTypeQueryValidator()
    {
        RuleFor(x => x.patientId)
            .GreaterThan(0).WithMessage("Patient ID must be greater than zero.");

        RuleFor(x => x.type)
            .IsInEnum().WithMessage("Invalid diagnosis type.");
    }
}