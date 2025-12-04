using FluentValidation;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Commands.MarkPurchaseInvoicePaid;

public class MarkPurchaseInvoicePaidValidator : AbstractValidator<MarkPurchaseInvoicePaidCommand>
{
    public MarkPurchaseInvoicePaidValidator()
    {
        RuleFor(x => x.PurchaseInvoiceId).GreaterThan(0);
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Method).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Reference).MaximumLength(200).When(x => !string.IsNullOrEmpty(x.Reference));
    }
}
