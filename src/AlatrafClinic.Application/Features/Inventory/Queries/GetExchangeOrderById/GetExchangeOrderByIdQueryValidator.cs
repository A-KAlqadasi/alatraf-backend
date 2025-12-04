using FluentValidation;

namespace AlatrafClinic.Application.Features.Inventory.Queries.GetExchangeOrderById;

public class GetExchangeOrderByIdQueryValidator : AbstractValidator<GetExchangeOrderByIdQuery>
{
    public GetExchangeOrderByIdQueryValidator()
    {
        RuleFor(x => x.ExchangeOrderId).GreaterThan(0);
    }
}
