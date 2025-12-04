using FluentValidation;

namespace AlatrafClinic.Application.Features.Inventory.Stores.Commands.AddItemUnitToStore;

public sealed class AddItemUnitToStoreCommandValidator : AbstractValidator<AddItemUnitToStoreCommand>
{
    public AddItemUnitToStoreCommandValidator()
    {
        RuleFor(x => x.StoreId)
            .GreaterThan(0).WithMessage("Store ID must be greater than zero.");

        RuleFor(x => x.ItemId)
            .GreaterThan(0).WithMessage("Item ID must be greater than zero.");

        RuleFor(x => x.UnitId)
            .GreaterThan(0).WithMessage("Unit ID must be greater than zero.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
    }
}
