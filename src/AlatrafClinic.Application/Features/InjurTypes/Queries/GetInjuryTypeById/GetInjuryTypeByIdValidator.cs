using FluentValidation;

namespace AlatrafClinic.Application.Features.InjurTypes.Queries.GetInjuryTypeById;

public class GetInjuryTypeByIdValidator : AbstractValidator<GetInjuryTypeByIdQuery>
{
    public GetInjuryTypeByIdValidator()
    {
        RuleFor(x=> x.InjuryTypeId)
            .GreaterThan(0).WithMessage("Injury type ID must be greater than zero.");
    }
}