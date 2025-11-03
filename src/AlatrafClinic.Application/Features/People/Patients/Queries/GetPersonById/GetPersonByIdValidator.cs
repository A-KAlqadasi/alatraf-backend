using FluentValidation;

namespace AlatrafClinic.Application.Features.People.Patients.Queries.GetPatientById;

public class GetPatientByIdQueryValidator : AbstractValidator<GetPatientByIdQuery>
{
    public GetPatientByIdQueryValidator()
    {
        RuleFor(q => q.PatientId)
            .GreaterThan(0)
            .NotEmpty()
            .WithMessage("PatientId required.");
    }
}
