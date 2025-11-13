using FluentValidation;

namespace AlatrafClinic.Application.Features.Inventory.Items.Queries.GetItemByIdQuery;

public class GetItemByIdQueryValidator : AbstractValidator<GetItemByIdQuery>
{
    public GetItemByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Item ID must be greater than 0.");
    }
}
