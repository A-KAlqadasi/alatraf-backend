using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Queries.GetPurchaseInvoiceItems;

public class GetPurchaseInvoiceItemsQueryHandler : IRequestHandler<GetPurchaseInvoiceItemsQuery, Result<List<AlatrafClinic.Application.Features.Inventory.Purchases.Dtos.PurchaseItemDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetPurchaseInvoiceItemsQueryHandler> _logger;

    public GetPurchaseInvoiceItemsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetPurchaseInvoiceItemsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<List<AlatrafClinic.Application.Features.Inventory.Purchases.Dtos.PurchaseItemDto>>> Handle(GetPurchaseInvoiceItemsQuery request, CancellationToken ct)
    {
        // Use repository projection to fetch items without loading the aggregate
        var projected = await _unitOfWork.PurchaseInvoices.GetItemsProjectedAsync(request.PurchaseInvoiceId, ct);
        if (projected == null)
        {
            _logger.LogInformation("No items found for PurchaseInvoice {Id}", request.PurchaseInvoiceId);
            return new List<AlatrafClinic.Application.Features.Inventory.Purchases.Dtos.PurchaseItemDto>();
        }

        return projected.ToList();
    }
}
