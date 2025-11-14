using FluentValidation;

namespace AlatrafClinic.Application.Features.Inventory.Suppliers.Queries.GetSupplierByIdQuery;

public sealed class GetSupplierByIdQueryValidator : AbstractValidator<GetSupplierByIdQuery>
{
    public GetSupplierByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Supplier ID must be greater than zero.");
    }
}
