using FluentValidation;

namespace AlatrafClinic.Application.Features.TherapyCards.Queries.GetPatientTherapyCards;

public class GetPatientTherapyCardsQueryValidator : AbstractValidator<GetPatientTherapyCardsQuery>
{
    public GetPatientTherapyCardsQueryValidator()
    {
        RuleFor(x=> x.PatientId)
            .GreaterThan(0).WithMessage("Patient id must be greater than zero");
    }
}