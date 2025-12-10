using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Queries.GetAllPurchaseInvoices;

public class GetAllPurchaseInvoicesQueryHandler : IRequestHandler<GetAllPurchaseInvoicesQuery, Result<List<AlatrafClinic.Application.Features.Inventory.Purchases.Dtos.PurchaseInvoiceDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetAllPurchaseInvoicesQueryHandler> _logger;

    public GetAllPurchaseInvoicesQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAllPurchaseInvoicesQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<List<AlatrafClinic.Application.Features.Inventory.Purchases.Dtos.PurchaseInvoiceDto>>> Handle(GetAllPurchaseInvoicesQuery request, CancellationToken ct)
    {
        // Use repository projection to avoid loading aggregates into memory.
        var projected = await _unitOfWork.PurchaseInvoices.GetAllProjectedAsync(ct);
        if (projected == null)
        {
            return new List<AlatrafClinic.Application.Features.Inventory.Purchases.Dtos.PurchaseInvoiceDto>();
        }

        return projected.ToList();
    }
}
