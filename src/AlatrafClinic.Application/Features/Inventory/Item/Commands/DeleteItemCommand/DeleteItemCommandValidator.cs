using AlatrafClinic.Application.Features.Inventory.Items.Commands.DeleteItemCommand;

using FluentValidation;

// namespace AlatrafClinic.Application.Features.Inventory.Item.Commands.DeleteItemCommand;

public class DeleteItemCommandValidator : AbstractValidator<DeleteItemCommand>
{
    public DeleteItemCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Item Id must be greater than zero.");
    }
}
