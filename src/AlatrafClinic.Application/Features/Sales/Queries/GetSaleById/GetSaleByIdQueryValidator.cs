using FluentValidation;

namespace AlatrafClinic.Application.Features.Sales.Queries.GetSaleById;

public class GetSaleByIdQueryValidator : AbstractValidator<GetSaleByIdQuery>
{
    public GetSaleByIdQueryValidator()
    {
        RuleFor(x => x.SaleId)
            .GreaterThan(0).WithMessage("SaleId must be greater than zero.");
    }
}