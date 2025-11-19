using AlatrafClinic.Application.Features.People.Patients.Queries.GetDisabledCardByNumber;

using FluentValidation;

namespace AlatrafClinic.Application.Features.People.Patients.Queries.GetWoundedCardByNumber;

public class GetWoundedCardByNumberQueryValidator : AbstractValidator<GetWoundedCardByNumberQuery>
{
    public GetWoundedCardByNumberQueryValidator()
    {
        RuleFor(x => x.CardNumber)
            .NotEmpty().WithMessage("Card number is required");
    }
}