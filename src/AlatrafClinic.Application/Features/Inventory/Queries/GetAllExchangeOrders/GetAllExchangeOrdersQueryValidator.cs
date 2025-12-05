using FluentValidation;

namespace AlatrafClinic.Application.Features.Inventory.Queries.GetAllExchangeOrders;

public class GetAllExchangeOrdersQueryValidator : AbstractValidator<GetAllExchangeOrdersQuery>
{
    public GetAllExchangeOrdersQueryValidator()
    {
        When(x => x.StoreId.HasValue, () => RuleFor(x => x.StoreId).GreaterThan(0));
        When(x => x.SaleId.HasValue, () => RuleFor(x => x.SaleId).GreaterThan(0));
        When(x => x.OrderId.HasValue, () => RuleFor(x => x.OrderId).GreaterThan(0));
        When(x => !string.IsNullOrWhiteSpace(x.SortDirection), () =>
        {
            RuleFor(x => x.SortDirection)
                .Must(d => d!.ToLower() == "asc" || d!.ToLower() == "desc")
                .WithMessage("SortDirection must be 'asc' or 'desc'.");
        });
    }
}
