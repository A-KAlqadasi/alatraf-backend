using System.Data;

using FluentValidation;

namespace AlatrafClinic.Application.Features.Sales.Commands.DeleteSale;

public class DeleteSaleCommandValidator : AbstractValidator<DeleteSaleCommand>
{
    public DeleteSaleCommandValidator()
    {
        RuleFor(x => x.SaleId)
            .GreaterThan(0).WithMessage("SaleId must be greater than zero.");
    }
}