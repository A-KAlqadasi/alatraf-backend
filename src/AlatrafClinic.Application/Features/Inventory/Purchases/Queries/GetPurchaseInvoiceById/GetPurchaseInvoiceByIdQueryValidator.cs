using FluentValidation;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Queries.GetPurchaseInvoiceById;

public class GetPurchaseInvoiceByIdQueryValidator : AbstractValidator<GetPurchaseInvoiceByIdQuery>
{
    public GetPurchaseInvoiceByIdQueryValidator()
    {
        RuleFor(x => x.PurchaseInvoiceId).GreaterThan(0);
    }
}
