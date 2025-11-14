using AlatrafClinic.Application.Features.Inventory.Items.Commands.DeactivateItemCommand;

using FluentValidation;

public class DeactivateItemCommandValidator : AbstractValidator<DeactivateItemCommand>
{
    public DeactivateItemCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Item Id must be greater than zero.");
    }
}
