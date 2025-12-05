using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Inventory.Purchases.Dtos;
using AlatrafClinic.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Queries.GetPurchaseInvoicesSummary;

public class GetPurchaseInvoicesSummaryQueryHandler : IRequestHandler<GetPurchaseInvoicesSummaryQuery, Result<List<PurchaseInvoiceSummaryDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetPurchaseInvoicesSummaryQueryHandler> _logger;

    public GetPurchaseInvoicesSummaryQueryHandler(IUnitOfWork unitOfWork, ILogger<GetPurchaseInvoicesSummaryQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<List<PurchaseInvoiceSummaryDto>>> Handle(GetPurchaseInvoicesSummaryQuery request, CancellationToken ct)
    {
        // Use repository projection to fetch lightweight DTOs, then apply filters in-memory.
        var projected = await _unitOfWork.PurchaseInvoices.GetAllProjectedAsync(ct);
        if (projected == null)
        {
            return new List<PurchaseInvoiceSummaryDto>();
        }

        var q = projected.AsQueryable();

        if (request.DateFrom.HasValue)
            q = q.Where(p => p.Date >= request.DateFrom.Value);
        if (request.DateTo.HasValue)
            q = q.Where(p => p.Date <= request.DateTo.Value);
        if (request.SupplierId.HasValue)
            q = q.Where(p => p.SupplierId == request.SupplierId.Value);
        if (request.StoreId.HasValue)
            q = q.Where(p => p.StoreId == request.StoreId.Value);

        var list = q.Select(p => new PurchaseInvoiceSummaryDto
        {
            Id = p.Id,
            Number = p.Number,
            Date = p.Date,
            SupplierName = p.SupplierName,
            StoreName = p.StoreName,
            Status = p.Status,
            TotalQuantities = p.TotalQuantities,
            TotalPrice = p.TotalPrice
        }).ToList();

        return list;
    }
}
