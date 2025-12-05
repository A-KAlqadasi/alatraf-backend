using FluentValidation;

namespace AlatrafClinic.Application.Features.Inventory.Queries.GetExchangeOrders;

public class GetExchangeOrdersQueryValidator : AbstractValidator<GetExchangeOrdersQuery>
{
    public GetExchangeOrdersQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 200);
        When(x => x.OrderId.HasValue, () => RuleFor(x => x.OrderId).GreaterThan(0));
        When(x => x.SaleId.HasValue, () => RuleFor(x => x.SaleId).GreaterThan(0));
        When(x => x.StoreId.HasValue, () => RuleFor(x => x.StoreId).GreaterThan(0));
        When(x => !string.IsNullOrWhiteSpace(x.SortDirection), () =>
        {
            RuleFor(x => x.SortDirection)
                .Must(d => d!.ToLower() == "asc" || d!.ToLower() == "desc")
                .WithMessage("SortDirection must be 'asc' or 'desc'.");
        });
    }
}
