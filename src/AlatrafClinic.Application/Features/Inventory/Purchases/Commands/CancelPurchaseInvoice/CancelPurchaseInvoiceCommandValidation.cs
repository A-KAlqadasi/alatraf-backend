using FluentValidation;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Commands.CancelPurchaseInvoice;

public sealed class CancelPurchaseInvoiceValidator : AbstractValidator<CancelPurchaseInvoiceCommand>
{
    public CancelPurchaseInvoiceValidator()
    {
        RuleFor(x => x.PurchaseInvoiceId).GreaterThan(0).WithMessage("Purchase invoice id is required.");
    }
}
