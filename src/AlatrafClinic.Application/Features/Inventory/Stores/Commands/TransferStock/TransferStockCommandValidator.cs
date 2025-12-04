using FluentValidation;

namespace AlatrafClinic.Application.Features.Inventory.Stores.Commands.TransferStock;

public sealed class TransferStockCommandValidator : AbstractValidator<TransferStockCommand>
{
    public TransferStockCommandValidator()
    {
        RuleFor(x => x.SourceStoreId).GreaterThan(0).WithMessage("Source store id must be greater than zero.");
        RuleFor(x => x.DestinationStoreId).GreaterThan(0).WithMessage("Destination store id must be greater than zero.");
        RuleFor(x => x.ItemId).GreaterThan(0).WithMessage("Item id must be greater than zero.");
        RuleFor(x => x.UnitId).GreaterThan(0).WithMessage("Unit id must be greater than zero.");
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        RuleFor(x => x).Must(x => x.SourceStoreId != x.DestinationStoreId).WithMessage("Source and destination stores must be different.");
    }
}
