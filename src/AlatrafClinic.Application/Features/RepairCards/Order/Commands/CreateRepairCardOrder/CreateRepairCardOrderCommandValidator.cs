using FluentValidation;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.CreateRepairCardOrder;

public class CreateRepairCardOrderCommandValidator : AbstractValidator<CreateRepairCardOrderCommand>
{
    public CreateRepairCardOrderCommandValidator()
    {
        RuleFor(x => x.RepairCardId).GreaterThan(0).WithMessage("RepairCardId is required.");
        RuleFor(x => x.SectionId).GreaterThan(0).WithMessage("SectionId is required.");
    }
}
