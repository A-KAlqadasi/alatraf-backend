using FluentValidation;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Commands.ApprovePurchaseInvoice;

public sealed class ApprovePurchaseInvoiceValidator : AbstractValidator<ApprovePurchaseInvoiceCommand>
{
    public ApprovePurchaseInvoiceValidator()
    {
        RuleFor(x => x.PurchaseInvoiceId).GreaterThan(0).WithMessage("Purchase invoice id is required.");
    }
}
