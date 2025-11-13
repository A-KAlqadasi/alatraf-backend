using AlatrafClinic.Application.Features.Inventory.Items.Commands.UpdateItemCommand;

using FluentValidation;

namespace AlatrafClinic.Application.Features.Inventory.Item.Commands;

public class UpdateItemCommandValidator : AbstractValidator<UpdateItemCommand>
{
    public UpdateItemCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Item Id must be greater than zero.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Item name is required.");
    }
}
