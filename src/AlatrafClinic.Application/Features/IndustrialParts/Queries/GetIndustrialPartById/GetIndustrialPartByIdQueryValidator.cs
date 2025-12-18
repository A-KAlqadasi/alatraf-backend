using FluentValidation;

namespace AlatrafClinic.Application.Features.IndustrialParts.Queries.GetIndustrialPartById;

public class GetIndustrialPartByIdQueryValidator : AbstractValidator<GetIndustrialPartByIdQuery>
{
    public GetIndustrialPartByIdQueryValidator()
    {
        RuleFor(x => x.IndustrialPartId)
            .GreaterThan(0).WithMessage("The Industrial Part Id must be greater than zero.");
    }
}