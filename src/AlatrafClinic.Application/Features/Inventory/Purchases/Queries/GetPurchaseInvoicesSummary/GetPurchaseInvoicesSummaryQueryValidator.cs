using FluentValidation;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Queries.GetPurchaseInvoicesSummary;

public class GetPurchaseInvoicesSummaryQueryValidator : AbstractValidator<GetPurchaseInvoicesSummaryQuery>
{
    public GetPurchaseInvoicesSummaryQueryValidator()
    {
        RuleFor(x => x)
            .Must(q => !(q.DateFrom.HasValue && q.DateTo.HasValue) || q.DateFrom.Value <= q.DateTo.Value)
            .WithMessage("DateFrom must be earlier than or equal to DateTo.");
        RuleFor(x => x.SupplierId).GreaterThan(0).When(x => x.SupplierId.HasValue);
        RuleFor(x => x.StoreId).GreaterThan(0).When(x => x.StoreId.HasValue);
    }
}
