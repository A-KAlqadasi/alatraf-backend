using FluentValidation;

namespace AlatrafClinic.Application.Features.Inventory.Commands.CreateExchangeOrderForSale;

public class CreateExchangeOrderForSaleCommandValidator : AbstractValidator<CreateExchangeOrderForSaleCommand>
{
    public CreateExchangeOrderForSaleCommandValidator()
    {
        RuleFor(x => x.SaleId).GreaterThan(0);
        RuleFor(x => x.StoreId).GreaterThan(0);
        RuleFor(x => x.Number).NotEmpty().WithMessage("Exchange order number is required.");
        RuleFor(x => x.Items).NotNull().Must(i => i.Count > 0).WithMessage("At least one item is required.");
        RuleForEach(x => x.Items).ChildRules(items =>
        {
            items.RuleFor(i => i.StoreItemUnitId).GreaterThan(0);
            items.RuleFor(i => i.Quantity).GreaterThan(0);
        });
    }
}
