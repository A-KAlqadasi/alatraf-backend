using FluentValidation;

namespace AlatrafClinic.Application.Features.Inventory.Units.Commands.CreateUnitCommand;

public class CreateUnitCommandValidator : AbstractValidator<CreateUnitCommand>
{
    public CreateUnitCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Unit name is required.")
            .MaximumLength(100).WithMessage("Unit name must be less than 100 characters.");
    }
}
