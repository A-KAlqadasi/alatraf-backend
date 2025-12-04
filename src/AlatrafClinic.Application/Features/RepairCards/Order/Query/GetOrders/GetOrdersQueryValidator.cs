using FluentValidation;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetOrders;

public class GetOrdersQueryValidator : AbstractValidator<GetOrdersQuery>
{
    public GetOrdersQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 200);
        When(x => x.SectionId.HasValue, () => RuleFor(x => x.SectionId).GreaterThan(0));
        When(x => x.RepairCardId.HasValue, () => RuleFor(x => x.RepairCardId).GreaterThan(0));
        When(x => !string.IsNullOrWhiteSpace(x.SortDirection), () =>
        {
            RuleFor(x => x.SortDirection)
                .Must(d => d!.ToLower() == "asc" || d!.ToLower() == "desc")
                .WithMessage("SortDirection must be 'asc' or 'desc'.");
        });
    }
}
