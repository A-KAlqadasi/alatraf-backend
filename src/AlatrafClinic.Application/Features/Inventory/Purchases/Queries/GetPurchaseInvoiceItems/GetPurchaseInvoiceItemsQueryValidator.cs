using FluentValidation;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Queries.GetPurchaseInvoiceItems;

public class GetPurchaseInvoiceItemsQueryValidator : AbstractValidator<GetPurchaseInvoiceItemsQuery>
{
    public GetPurchaseInvoiceItemsQueryValidator()
    {
        RuleFor(x => x.PurchaseInvoiceId).GreaterThan(0);
    }
}
