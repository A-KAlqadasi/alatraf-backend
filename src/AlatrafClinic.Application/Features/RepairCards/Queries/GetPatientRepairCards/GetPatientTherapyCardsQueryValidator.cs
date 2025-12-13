
using FluentValidation;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetPatientRepairCards;

public class GetPatientRepairCardsQueryValidator : AbstractValidator<GetPatientRepairCardsQuery>
{
    public GetPatientRepairCardsQueryValidator()
    {
        RuleFor(x=> x.PatientId)
            .GreaterThan(0).WithMessage("Patient id must be greater than zero");
    }
}