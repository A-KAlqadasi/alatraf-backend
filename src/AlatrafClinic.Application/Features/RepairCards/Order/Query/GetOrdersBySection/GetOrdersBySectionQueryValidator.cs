using FluentValidation;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetOrdersBySection;

public sealed class GetOrdersBySectionQueryValidator : AbstractValidator<GetOrdersBySectionQuery>
{
    public GetOrdersBySectionQueryValidator()
    {
        RuleFor(x => x.SectionId).GreaterThan(0).WithMessage("Section id is required.");
    }
}
