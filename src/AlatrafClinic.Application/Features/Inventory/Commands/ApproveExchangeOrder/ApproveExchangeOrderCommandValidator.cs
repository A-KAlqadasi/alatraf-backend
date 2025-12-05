using FluentValidation;

namespace AlatrafClinic.Application.Features.Inventory.Commands.ApproveExchangeOrder;

public class ApproveExchangeOrderCommandValidator : AbstractValidator<ApproveExchangeOrderCommand>
{
    public ApproveExchangeOrderCommandValidator()
    {
        RuleFor(x => x.ExchangeOrderId).GreaterThan(0);
    }
}
