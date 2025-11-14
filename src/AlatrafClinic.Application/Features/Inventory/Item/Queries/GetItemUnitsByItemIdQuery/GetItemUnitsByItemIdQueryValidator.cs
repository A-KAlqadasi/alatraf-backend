using FluentValidation;

namespace AlatrafClinic.Application.Features.Inventory.Items.Queries.GetItemUnitsByItemIdQuery;

public class GetItemUnitsByItemIdQueryValidator : AbstractValidator<GetItemUnitsByItemIdQuery>
{
    public GetItemUnitsByItemIdQueryValidator()
    {
        RuleFor(x => x.ItemId)
            .GreaterThan(0).WithMessage("ItemId must be greater than 0.");
    }
}
