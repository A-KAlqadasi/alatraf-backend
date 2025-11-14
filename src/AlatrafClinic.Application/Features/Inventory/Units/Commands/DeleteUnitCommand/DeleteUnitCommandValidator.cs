using FluentValidation;

namespace AlatrafClinic.Application.Features.Inventory.Units.Commands.DeleteUnitCommand;

public class DeleteUnitCommandValidator : AbstractValidator<DeleteUnitCommand>
{
    public DeleteUnitCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Invalid Unit ID.");
    }
}
