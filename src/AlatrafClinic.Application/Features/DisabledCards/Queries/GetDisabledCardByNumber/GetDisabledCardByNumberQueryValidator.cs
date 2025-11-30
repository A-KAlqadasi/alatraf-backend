using FluentValidation;

namespace AlatrafClinic.Application.Features.DisabledCards.Queries.GetDisabledCardByNumber;

public class GetDisabledCardByNumberQueryValidator : AbstractValidator<GetDisabledCardByNumberQuery>
{
    public GetDisabledCardByNumberQueryValidator()
    {
        RuleFor(x => x.CardNumber)
            .NotEmpty().WithMessage("Card number is required");
    }
}