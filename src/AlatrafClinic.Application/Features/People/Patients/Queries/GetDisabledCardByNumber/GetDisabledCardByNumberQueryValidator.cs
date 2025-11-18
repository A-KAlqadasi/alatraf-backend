using FluentValidation;

namespace AlatrafClinic.Application.Features.People.Patients.Queries.GetDisabledCardByNumber;

public class GetDisabledCardByNumberQueryValidator : AbstractValidator<GetDisabledCardByNumberQuery>
{
    public GetDisabledCardByNumberQueryValidator()
    {
        RuleFor(x => x.CardNumber)
            .NotEmpty().WithMessage("Card number is required");
    }
}