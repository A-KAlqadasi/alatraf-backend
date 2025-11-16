using FluentValidation;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.CreateRepairCard;

public class CreateRepairCardIndustrialPartCommandValidator : AbstractValidator<CreateRepairCardIndustrialPartCommand>
{
    public CreateRepairCardIndustrialPartCommandValidator()
    {
        RuleFor(x => x.IndustrialPartId).GreaterThan(0);
        RuleFor(x => x.UnitId).GreaterThan(0);
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
    }
}