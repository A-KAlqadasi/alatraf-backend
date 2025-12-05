using FluentValidation;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.UpdateOrderSection;

public class UpdateOrderSectionCommandValidator : AbstractValidator<UpdateOrderSectionCommand>
{
    public UpdateOrderSectionCommandValidator()
    {
        RuleFor(x => x.RepairCardId).GreaterThan(0).WithMessage("RepairCardId is required.");
        RuleFor(x => x.OrderId).GreaterThan(0).WithMessage("OrderId is required.");
        RuleFor(x => x.SectionId).GreaterThan(0).WithMessage("SectionId is required.");
    }
}
