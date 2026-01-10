using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Purchases.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Queries.GetAllPurchaseInvoices;

public class GetAllPurchaseInvoicesQueryHandler : IRequestHandler<GetAllPurchaseInvoicesQuery, Result<List<AlatrafClinic.Application.Features.Inventory.Purchases.Dtos.PurchaseInvoiceDto>>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<GetAllPurchaseInvoicesQueryHandler> _logger;

    public GetAllPurchaseInvoicesQueryHandler(IAppDbContext dbContext, ILogger<GetAllPurchaseInvoicesQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<List<AlatrafClinic.Application.Features.Inventory.Purchases.Dtos.PurchaseInvoiceDto>>> Handle(GetAllPurchaseInvoicesQuery request, CancellationToken ct)
    {
        var projected = await _dbContext.PurchaseInvoices
            .AsNoTracking()
            .Select(p => new PurchaseInvoiceDto
            {
                Id = p.Id,
                Number = p.Number,
                Date = p.Date,
                SupplierId = p.SupplierId,
                SupplierName = p.Supplier.SupplierName,
                StoreId = p.StoreId,
                StoreName = p.Store.Name,
                Status = p.Status.ToString(),
                PostedAtUtc = p.PostedAtUtc,
                PaidAtUtc = p.PaidAtUtc,
                PaymentAmount = p.PaymentAmount,
                PaymentMethod = p.PaymentMethod,
                PaymentReference = p.PaymentReference,
                TotalQuantities = p.Items.Sum(i => i.Quantity),
                TotalPrice = p.Items.Sum(i => i.Total)
            })
            .ToListAsync(ct);

        return projected;
    }
}
