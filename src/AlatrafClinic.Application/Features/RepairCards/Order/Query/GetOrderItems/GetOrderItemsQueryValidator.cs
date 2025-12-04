using FluentValidation;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetOrderItems;

public sealed class GetOrderItemsQueryValidator : AbstractValidator<GetOrderItemsQuery>
{
    public GetOrderItemsQueryValidator()
    {
        RuleFor(x => x.OrderId).GreaterThan(0).WithMessage("Order id is required.");
    }
}
