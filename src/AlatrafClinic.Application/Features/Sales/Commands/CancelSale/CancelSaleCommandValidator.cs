using System.Data;

using FluentValidation;

namespace AlatrafClinic.Application.Features.Sales.Commands.CancelSale;

public class CancelSaleCommandValidator : AbstractValidator<CancelSaleCommand>
{
    public CancelSaleCommandValidator()
    {
        RuleFor(x => x.SaleId)
            .GreaterThan(0).WithMessage("SaleId must be greater than zero.");
    }
}