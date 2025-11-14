using FluentValidation;

namespace AlatrafClinic.Application.Features.Inventory.Items.Commands.CreateItemCommand;

public class CreateItemCommandValidator : AbstractValidator<CreateItemCommand>
{
    public CreateItemCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Item name is required");

        RuleFor(x => x.BaseUnitId)
            .GreaterThan(0).WithMessage("Base unit must be selected");

        RuleFor(x => x.Units)
            .NotEmpty().WithMessage("At least one unit is required")
            .Must(units => units.Select(u => u.UnitId).Distinct().Count() == units.Count)
            .WithMessage("Unit IDs must be unique");

        RuleForEach(x => x.Units).ChildRules(unit =>
        {
            unit.RuleFor(u => u.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price must be >= 0");

            unit.RuleFor(u => u.ConversionFactor)
                .GreaterThan(0).WithMessage("Conversion factor must be > 0");
        });
    }
}
