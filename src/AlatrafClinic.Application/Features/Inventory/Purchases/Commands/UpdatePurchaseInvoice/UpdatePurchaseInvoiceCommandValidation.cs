using FluentValidation;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Commands.UpdatePurchaseInvoice;

public sealed class UpdatePurchaseInvoiceValidator : AbstractValidator<UpdatePurchaseInvoiceCommand>
{
    public UpdatePurchaseInvoiceValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Invoice id is required.");
        RuleFor(x => x.Number).NotEmpty().WithMessage("Invoice number is required.");
        RuleFor(x => x.SupplierId).GreaterThan(0).WithMessage("Supplier is required.");
        RuleFor(x => x.StoreId).GreaterThan(0).WithMessage("Store is required.");
        RuleFor(x => x.Date).NotEmpty().WithMessage("Date is required.");
    }
}
