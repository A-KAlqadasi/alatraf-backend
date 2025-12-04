using FluentValidation;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Commands.CreatePurchaseInvoice;

public sealed class CreatePurchaseInvoiceValidator : AbstractValidator<CreatePurchaseInvoiceCommand>
{
    public CreatePurchaseInvoiceValidator()
    {
        RuleFor(x => x.Number).NotEmpty().WithMessage("Invoice number is required.");
        RuleFor(x => x.SupplierId).GreaterThan(0).WithMessage("Supplier is required.");
        RuleFor(x => x.StoreId).GreaterThan(0).WithMessage("Store is required.");
        RuleFor(x => x.Date).NotEmpty().WithMessage("Date is required.");
    }
}
