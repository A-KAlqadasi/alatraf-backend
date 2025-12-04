using FluentValidation;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Commands.SubmitPurchaseInvoice;

public sealed class SubmitPurchaseInvoiceValidator : AbstractValidator<SubmitPurchaseInvoiceCommand>
{
    public SubmitPurchaseInvoiceValidator()
    {
        RuleFor(x => x.PurchaseInvoiceId).GreaterThan(0).WithMessage("Purchase invoice id is required.");
    }
}
