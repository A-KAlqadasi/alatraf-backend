using FluentValidation;

namespace AlatrafClinic.Application.Features.Inventory.Stores.Queries.GetStoreItemUnitsQuery;

public class GetStoreItemUnitsQueryValidator : AbstractValidator<GetStoreItemUnitsQuery>
{
    public GetStoreItemUnitsQueryValidator()
    {
        RuleFor(x => x.StoreId)
            .GreaterThan(0).WithMessage("Store ID must be greater than zero.");
    }
}
