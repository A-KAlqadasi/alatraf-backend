using FluentValidation;

namespace AlatrafClinic.Application.Features.Inventory.Units.Commands.UpdateUnitCommand;

public class UpdateUnitCommandValidator : AbstractValidator<UpdateUnitCommand>
{
    public UpdateUnitCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Invalid Unit ID.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Unit name is required.")
            .MaximumLength(100).WithMessage("Unit name must be less than 100 characters.");
    }
}
