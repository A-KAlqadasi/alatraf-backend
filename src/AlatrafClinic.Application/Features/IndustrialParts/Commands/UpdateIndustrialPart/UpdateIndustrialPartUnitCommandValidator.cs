using FluentValidation;

namespace AlatrafClinic.Application.Features.IndustrialParts.Commands.UpdateIndustrialPart;

public class UpdateIndustrialPartUnitCommandValidator : AbstractValidator<UpdateIndustrialPartUnitCommand>
{
    public UpdateIndustrialPartUnitCommandValidator()
    {
        RuleFor(x => x.UnitId)
            .GreaterThan(0).WithMessage("UnitId must be greater than zero.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");
    }
}