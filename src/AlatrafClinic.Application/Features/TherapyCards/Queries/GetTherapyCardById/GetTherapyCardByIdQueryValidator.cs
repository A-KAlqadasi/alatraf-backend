using FluentValidation;

namespace AlatrafClinic.Application.Features.TherapyCards.Queries.GetTherapyCardById;

public class GetTherapyCardByIdQueryValidator : AbstractValidator<GetTherapyCardByIdQuery>
{
    public GetTherapyCardByIdQueryValidator()
    {
        RuleFor(x => x.TherapyCardId)
            .GreaterThan(0).WithMessage("Therapy card Id is invalid");
    }
}