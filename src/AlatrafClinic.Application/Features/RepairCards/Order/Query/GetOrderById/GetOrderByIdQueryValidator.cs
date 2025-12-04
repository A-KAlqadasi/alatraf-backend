using FluentValidation;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetOrderById;

public sealed class GetOrderByIdQueryValidator : AbstractValidator<GetOrderByIdQuery>
{
    public GetOrderByIdQueryValidator()
    {
        RuleFor(x => x.OrderId).GreaterThan(0).WithMessage("Order id is required.");
    }
}
