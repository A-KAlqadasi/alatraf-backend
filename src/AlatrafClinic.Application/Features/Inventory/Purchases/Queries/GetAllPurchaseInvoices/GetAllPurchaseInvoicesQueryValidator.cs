using FluentValidation;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Queries.GetAllPurchaseInvoices;

public class GetAllPurchaseInvoicesQueryValidator : AbstractValidator<GetAllPurchaseInvoicesQuery>
{
    public GetAllPurchaseInvoicesQueryValidator()
    {
        // No parameters to validate for now; keep validator for DI consistency
    }
}
