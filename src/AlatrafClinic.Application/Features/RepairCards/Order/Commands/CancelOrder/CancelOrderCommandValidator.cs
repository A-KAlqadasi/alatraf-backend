using FluentValidation;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.CancelOrder;

public class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand>
{
    public CancelOrderCommandValidator()
    {
        RuleFor(x => x.OrderId).GreaterThan(0).WithMessage("OrderId is required.");
    }
}
