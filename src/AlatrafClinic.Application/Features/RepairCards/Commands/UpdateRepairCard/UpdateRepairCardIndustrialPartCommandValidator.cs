using FluentValidation;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.UpdateRepairCard;

public class UpdateRepairCardIndustrialPartCommandValidator : AbstractValidator<UpdateRepairCardIndustrialPartCommand>
{
    public UpdateRepairCardIndustrialPartCommandValidator()
    {
        RuleFor(x => x.IndustrialPartId).GreaterThan(0);
        RuleFor(x => x.UnitId).GreaterThan(0);
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}