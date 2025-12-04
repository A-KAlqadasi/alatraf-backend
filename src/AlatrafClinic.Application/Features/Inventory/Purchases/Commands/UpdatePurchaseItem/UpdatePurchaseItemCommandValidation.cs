using FluentValidation;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Commands.UpdatePurchaseItem;

public sealed class UpdatePurchaseItemValidator : AbstractValidator<UpdatePurchaseItemCommand>
{
    public UpdatePurchaseItemValidator()
    {
        RuleFor(x => x.PurchaseInvoiceId).GreaterThan(0).WithMessage("Purchase invoice id is required.");
        RuleFor(x => x.ExistingStoreItemUnitId).GreaterThan(0).WithMessage("Existing store item unit id is required.");
        RuleFor(x => x.NewStoreItemUnitId).GreaterThan(0).WithMessage("New store item unit id is required.");
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        RuleFor(x => x.UnitPrice).GreaterThan(0).WithMessage("Unit price must be greater than zero.");
    }
}
