using FluentValidation;

namespace AlatrafClinic.Application.Features.InjuryReasons.Queries.GetInjuryReasonById;

public class GetInjuryReasonByIdValidator : AbstractValidator<GetInjuryReasonByIdQuery>
{
    public GetInjuryReasonByIdValidator()
    {
        RuleFor(x=> x.InjuryReasonId)
            .GreaterThan(0).WithMessage("Injury reason ID must be greater than zero.");
    }
}