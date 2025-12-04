using FluentValidation;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Commands.RemovePurchaseItem;

public sealed class RemovePurchaseItemValidator : AbstractValidator<RemovePurchaseItemCommand>
{
    public RemovePurchaseItemValidator()
    {
        RuleFor(x => x.PurchaseInvoiceId).GreaterThan(0).WithMessage("Purchase invoice id is required.");
        RuleFor(x => x.StoreItemUnitId).GreaterThan(0).WithMessage("Store item unit id is required.");
    }
}
