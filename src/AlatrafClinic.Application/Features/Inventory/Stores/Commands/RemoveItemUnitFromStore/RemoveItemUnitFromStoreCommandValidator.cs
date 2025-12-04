using FluentValidation;

namespace AlatrafClinic.Application.Features.Inventory.Stores.Commands.RemoveItemUnitFromStore;

public sealed class RemoveItemUnitFromStoreCommandValidator : AbstractValidator<RemoveItemUnitFromStoreCommand>
{
    public RemoveItemUnitFromStoreCommandValidator()
    {
        RuleFor(x => x.StoreId)
            .GreaterThan(0).WithMessage("Store ID must be greater than zero.");

        RuleFor(x => x.ItemId)
            .GreaterThan(0).WithMessage("Item ID must be greater than zero.");

        RuleFor(x => x.UnitId)
            .GreaterThan(0).WithMessage("Unit ID must be greater than zero.");
    }
}
