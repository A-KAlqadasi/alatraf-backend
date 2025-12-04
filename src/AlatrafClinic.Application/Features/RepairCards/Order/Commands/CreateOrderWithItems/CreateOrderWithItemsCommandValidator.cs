using FluentValidation;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.CreateOrderWithItems;

public class CreateOrderWithItemsCommandValidator : AbstractValidator<CreateOrderWithItemsCommand>
{
    public CreateOrderWithItemsCommandValidator()
    {
        RuleFor(x => x.SectionId).GreaterThan(0).WithMessage("SectionId is required.");
        RuleFor(x => x.Items).NotNull().WithMessage("Items are required.").Must(i => i.Count > 0).WithMessage("At least one item is required.");
        RuleForEach(x => x.Items).ChildRules(items =>
        {
            items.RuleFor(i => i.ItemId).GreaterThan(0).WithMessage("ItemId is required.");
            items.RuleFor(i => i.UnitId).GreaterThan(0).WithMessage("UnitId is required.");
            items.RuleFor(i => i.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        });
    }
}
