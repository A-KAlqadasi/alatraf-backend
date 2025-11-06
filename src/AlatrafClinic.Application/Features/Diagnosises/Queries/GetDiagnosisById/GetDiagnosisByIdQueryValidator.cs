using FluentValidation;

namespace AlatrafClinic.Application.Features.Diagnosises.Queries.GetDiagnosisById;

public class GetDiagnosisByIdQueryValidator : AbstractValidator<GetDiagnosisByIdQuery>
{
    public GetDiagnosisByIdQueryValidator()
    {
        RuleFor(x => x.diagnosisId)
            .GreaterThan(0).WithMessage("Diagnosis Id must be greater than zero.");
    }
}