using FluentValidation;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.CreateDeliveryTime;

public class CreateDeliveryTimeCommandValidator : AbstractValidator<CreateDeliveryTimeCommand>
{
    public CreateDeliveryTimeCommandValidator()
    {
        RuleFor(x => x.RepairCardId)
            .GreaterThan(0).WithMessage("Repair card Id is invalid");
        RuleFor(x => x.DeliveryDate)
            .GreaterThan(DateTime.Now.Date).WithMessage("Delivery date must be in future");
    }
}