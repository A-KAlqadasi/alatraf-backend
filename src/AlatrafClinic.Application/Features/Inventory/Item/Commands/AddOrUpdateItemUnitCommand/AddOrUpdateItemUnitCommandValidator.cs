using AlatrafClinic.Application.Features.Inventory.Items.Commands.AddOrUpdateItemUnitCommand;

using FluentValidation;


public class AddOrUpdateItemUnitCommandValidator : AbstractValidator<AddOrUpdateItemUnitCommand>
{
    public AddOrUpdateItemUnitCommandValidator()
    {
        RuleFor(x => x.ItemId)
            .GreaterThan(0).WithMessage("Item Id must be greater than zero.");

        RuleFor(x => x.UnitId)
            .GreaterThan(0).WithMessage("Unit Id must be greater than zero.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be greater than or equal to zero.");

        RuleFor(x => x.ConversionFactor)
            .GreaterThan(0).WithMessage("ConversionFactor must be greater than zero.");
    }
}
