using FluentValidation;

namespace AlatrafClinic.Application.Features.Inventory.Stores.Queries.GetStoreByIdQuery;

public class GetStoreByIdQueryValidator : AbstractValidator<GetStoreByIdQuery>
{
    public GetStoreByIdQueryValidator()
    {
        RuleFor(x => x.StoreId)
            .GreaterThan(0).WithMessage("Store ID must be greater than zero.");
    }
}
