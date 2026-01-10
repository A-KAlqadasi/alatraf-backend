using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Purchases.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Queries.GetPurchaseInvoicesSummary;

public class GetPurchaseInvoicesSummaryQueryHandler : IRequestHandler<GetPurchaseInvoicesSummaryQuery, Result<List<PurchaseInvoiceSummaryDto>>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<GetPurchaseInvoicesSummaryQueryHandler> _logger;

    public GetPurchaseInvoicesSummaryQueryHandler(IAppDbContext dbContext, ILogger<GetPurchaseInvoicesSummaryQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<List<PurchaseInvoiceSummaryDto>>> Handle(GetPurchaseInvoicesSummaryQuery request, CancellationToken ct)
    {
        var query = _dbContext.PurchaseInvoices
            .AsNoTracking()
            .AsQueryable();

        if (request.DateFrom.HasValue)
            query = query.Where(p => p.Date >= request.DateFrom.Value);
        if (request.DateTo.HasValue)
            query = query.Where(p => p.Date <= request.DateTo.Value);
        if (request.SupplierId.HasValue)
            query = query.Where(p => p.SupplierId == request.SupplierId.Value);
        if (request.StoreId.HasValue)
            query = query.Where(p => p.StoreId == request.StoreId.Value);

        var list = await query
            .Select(p => new PurchaseInvoiceSummaryDto
            {
                Id = p.Id,
                Number = p.Number,
                Date = p.Date,
                SupplierName = p.Supplier.SupplierName,
                StoreName = p.Store.Name,
                Status = p.Status.ToString(),
                TotalQuantities = p.Items.Sum(i => i.Quantity),
                TotalPrice = p.Items.Sum(i => i.Total)
            })
            .ToListAsync(ct);

        return list;
    }
}
