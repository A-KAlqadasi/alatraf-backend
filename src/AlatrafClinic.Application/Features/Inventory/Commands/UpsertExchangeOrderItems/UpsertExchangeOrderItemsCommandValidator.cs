using FluentValidation;

namespace AlatrafClinic.Application.Features.Inventory.Commands.UpsertExchangeOrderItems;

public class UpsertExchangeOrderItemsCommandValidator : AbstractValidator<UpsertExchangeOrderItemsCommand>
{
    public UpsertExchangeOrderItemsCommandValidator()
    {
        RuleFor(x => x.ExchangeOrderId).GreaterThan(0);
        RuleFor(x => x.Items).NotNull().Must(i => i.Count > 0).WithMessage("At least one item is required.");
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.StoreItemUnitId).GreaterThan(0);
            item.RuleFor(i => i.Quantity).GreaterThan(0);
        });
    }
}
