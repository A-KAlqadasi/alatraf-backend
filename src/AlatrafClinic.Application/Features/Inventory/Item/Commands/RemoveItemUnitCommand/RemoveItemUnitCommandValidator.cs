using AlatrafClinic.Application.Features.Inventory.Items.Commands.RemoveItemUnitCommand;

using FluentValidation;


public class RemoveItemUnitCommandValidator : AbstractValidator<RemoveItemUnitCommand>
{
    public RemoveItemUnitCommandValidator()
    {
        RuleFor(x => x.ItemId)
            .GreaterThan(0).WithMessage("Item Id must be greater than zero.");

        RuleFor(x => x.UnitId)
            .GreaterThan(0).WithMessage("Unit Id must be greater than zero.");
    }
}
