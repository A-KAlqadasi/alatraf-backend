using FluentValidation;

namespace AlatrafClinic.Application.Features.WoundedCards.Queries.GetWoundedCardByNumber;

public class GetWoundedCardByNumberQueryValidator : AbstractValidator<GetWoundedCardByNumberQuery>
{
    public GetWoundedCardByNumberQueryValidator()
    {
        RuleFor(x => x.CardNumber)
            .NotEmpty().WithMessage("Card number is required");
    }
}