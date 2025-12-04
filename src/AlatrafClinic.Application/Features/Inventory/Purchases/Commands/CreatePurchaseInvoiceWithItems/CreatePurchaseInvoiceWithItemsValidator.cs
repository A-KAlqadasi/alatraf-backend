using FluentValidation;
using System.Linq;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Commands.CreatePurchaseInvoiceWithItems;

public class CreatePurchaseInvoiceWithItemsValidator : AbstractValidator<CreatePurchaseInvoiceWithItemsCommand>
{
	public CreatePurchaseInvoiceWithItemsValidator()
	{
		RuleFor(x => x.Number)
			.NotEmpty()
			.MaximumLength(100);

		RuleFor(x => x.Date)
			.Must(d => d <= DateTime.UtcNow.AddDays(1))
			.WithMessage("Date cannot be in the future.");

		RuleFor(x => x.SupplierId).GreaterThan(0);
		RuleFor(x => x.StoreId).GreaterThan(0);

		RuleFor(x => x.Items)
			.NotNull()
			.Must(it => it.Any())
			.WithMessage("At least one item is required.");

		RuleFor(x => x.Items)
			.Must(items => items == null || items.Select(i => i.StoreItemUnitId).Distinct().Count() == items.Count())
			.WithMessage("Duplicate store item units are not allowed.");

		RuleForEach(x => x.Items).ChildRules(items => {
			items.RuleFor(i => i.StoreItemUnitId).GreaterThan(0);
			items.RuleFor(i => i.Quantity).GreaterThan(0);
			items.RuleFor(i => i.UnitPrice).GreaterThanOrEqualTo(0);
			items.RuleFor(i => i.Notes).MaximumLength(500).When(i => !string.IsNullOrEmpty(i.Notes));
		});
	}
}
