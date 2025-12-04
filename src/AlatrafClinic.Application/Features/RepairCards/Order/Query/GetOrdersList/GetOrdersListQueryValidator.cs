using FluentValidation;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetOrdersList;

public sealed class GetOrdersListQueryValidator : AbstractValidator<GetOrdersListQuery>
{
    public GetOrdersListQueryValidator()
    {
        // No parameters to validate currently; validator exists for extensibility.
    }
}
