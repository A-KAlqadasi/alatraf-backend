using FluentValidation;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.UpdateRepairCard;

public class UpdateRepairCardIndustrialPartCommandValidator : AbstractValidator<UpdateRepairCardIndustrialPartCommand>
{
    public UpdateRepairCardIndustrialPartCommandValidator()
    {
        RuleFor(x => x.IndustrialPartId).GreaterThan(0).WithMessage("IndustrialPartId must be greater than 0.");
        RuleFor(x => x.UnitId).GreaterThan(0).WithMessage("UnitId must be greater than 0.");
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than 0.");
    }
}