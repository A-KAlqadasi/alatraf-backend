using FluentValidation;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.UpsertOrderItems;

public class UpsertOrderItemsCommandValidator : AbstractValidator<UpsertOrderItemsCommand>
{
    public UpsertOrderItemsCommandValidator()
    {
        RuleFor(x => x.RepairCardId).GreaterThan(0).WithMessage("RepairCardId is required.");
        RuleFor(x => x.OrderId).GreaterThan(0).WithMessage("OrderId is required.");
        RuleFor(x => x.Items).NotNull().WithMessage("Items are required.");
        RuleForEach(x => x.Items).ChildRules(items =>
        {
            items.RuleFor(i => i.ItemId).GreaterThan(0).WithMessage("ItemId is required.");
            items.RuleFor(i => i.UnitId).GreaterThan(0).WithMessage("UnitId is required.");
            items.RuleFor(i => i.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        });
    }
}
