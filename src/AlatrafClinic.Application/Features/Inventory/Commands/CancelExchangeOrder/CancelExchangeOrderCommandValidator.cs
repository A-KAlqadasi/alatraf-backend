using FluentValidation;

namespace AlatrafClinic.Application.Features.Inventory.Commands.CancelExchangeOrder;

public class CancelExchangeOrderCommandValidator : AbstractValidator<CancelExchangeOrderCommand>
{
    public CancelExchangeOrderCommandValidator()
    {
        RuleFor(x => x.ExchangeOrderId).GreaterThan(0);
    }
}
