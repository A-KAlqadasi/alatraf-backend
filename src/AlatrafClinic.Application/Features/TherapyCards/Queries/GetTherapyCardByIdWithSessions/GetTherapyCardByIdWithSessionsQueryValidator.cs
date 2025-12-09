using FluentValidation;

namespace AlatrafClinic.Application.Features.TherapyCards.Queries.GetTherapyCardByIdWithSessions;

public class GetTherapyCardByIdWithSessionsQueryValidator : AbstractValidator<GetTherapyCardByIdWithSessionsQuery>
{
    public GetTherapyCardByIdWithSessionsQueryValidator()
    {
        RuleFor(x => x.TherapyCardId)
            .GreaterThan(0).WithMessage("Therapy card Id is invalid");
    }
}