using System.Data;

using FluentValidation;

namespace AlatrafClinic.Application.Features.Diagnosises.Queries.GetPatientDiagnoses;

public class GetPatientDiagnosesQueryValidator : AbstractValidator<GetPatientDiagnosesQuery>
{
    public GetPatientDiagnosesQueryValidator()
    {
        RuleFor(x => x.patientId)
            .GreaterThan(0).WithMessage("Patient ID must be greater than zero.");
    }
}