using FluentValidation;

namespace AlatrafClinic.Application.Features.InjurySides.Queries.GetInjurySideById;

public class GetInjurySideByIdValidator : AbstractValidator<GetInjurySideByIdQuery>
{
    public GetInjurySideByIdValidator()
    {
        RuleFor(x=> x.InjurySideId)
            .GreaterThan(0).WithMessage("Injury side ID must be greater than zero.");
    }
}