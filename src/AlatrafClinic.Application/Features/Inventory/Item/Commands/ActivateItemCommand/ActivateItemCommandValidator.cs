using AlatrafClinic.Application.Features.Inventory.Items.Commands.ActivateItemCommand;

using FluentValidation;


public class ActivateItemCommandValidator : AbstractValidator<ActivateItemCommand>
{
    public ActivateItemCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Item Id must be greater than zero.");
    }
}
