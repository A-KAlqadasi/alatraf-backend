using FluentValidation;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetDraftOrders;

public sealed class GetDraftOrdersQueryValidator : AbstractValidator<GetDraftOrdersQuery>
{
    public GetDraftOrdersQueryValidator()
    {
        // No parameters to validate currently; validator exists for extensibility.
    }
}
