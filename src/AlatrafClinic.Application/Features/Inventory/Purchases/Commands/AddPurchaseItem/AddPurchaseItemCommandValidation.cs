using FluentValidation;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Commands.AddPurchaseItem;

public sealed class AddPurchaseItemValidator : AbstractValidator<AddPurchaseItemCommand>
{
    public AddPurchaseItemValidator()
    {
        RuleFor(x => x.PurchaseInvoiceId).GreaterThan(0).WithMessage("Purchase invoice id is required.");
        RuleFor(x => x.StoreItemUnitId).GreaterThan(0).WithMessage("Store item unit id is required.");
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        RuleFor(x => x.UnitPrice).GreaterThan(0).WithMessage("Unit price must be greater than zero.");
    }
}
