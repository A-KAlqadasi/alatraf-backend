using FluentValidation;

namespace AlatrafClinic.Application.Features.TherapyCards.Queries.GetLastActiveTherapyCard;

public class GetLastActiveTherapyCardQueryValidator : AbstractValidator<GetLastActiveTherapyCardQuery>
{
    public GetLastActiveTherapyCardQueryValidator()
    {
        RuleFor(x => x.PatientId)
            .GreaterThan(0).WithMessage("Patient ID must be greater than zero.");
    }
}