using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Purchases.Dtos;
using AlatrafClinic.Application.Features.Inventory.Purchases.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Queries.GetPurchaseInvoiceById;

public class GetPurchaseInvoiceByIdQueryHandler : IRequestHandler<GetPurchaseInvoiceByIdQuery, Result<AlatrafClinic.Application.Features.Inventory.Purchases.Dtos.PurchaseInvoiceDto>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<GetPurchaseInvoiceByIdQueryHandler> _logger;

    public GetPurchaseInvoiceByIdQueryHandler(IAppDbContext dbContext, ILogger<GetPurchaseInvoiceByIdQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<AlatrafClinic.Application.Features.Inventory.Purchases.Dtos.PurchaseInvoiceDto>> Handle(GetPurchaseInvoiceByIdQuery request, CancellationToken ct)
    {
        var invoiceDto = await _dbContext.PurchaseInvoices
            .AsNoTracking()
            .Where(i => i.Id == request.PurchaseInvoiceId)
            .Select(i => new PurchaseInvoiceDto
            {
                Id = i.Id,
                Number = i.Number,
                Date = i.Date,
                SupplierId = i.SupplierId,
                SupplierName = i.Supplier.SupplierName,
                StoreId = i.StoreId,
                StoreName = i.Store.Name,
                Status = i.Status.ToString(),
                PostedAtUtc = i.PostedAtUtc,
                PaidAtUtc = i.PaidAtUtc,
                PaymentAmount = i.PaymentAmount,
                PaymentMethod = i.PaymentMethod,
                PaymentReference = i.PaymentReference,
                TotalQuantities = i.Items.Sum(item => item.Quantity),
                TotalPrice = i.Items.Sum(item => item.Total)
            })
            .SingleOrDefaultAsync(ct);

        if (invoiceDto is null)
        {
            _logger.LogWarning("PurchaseInvoice {Id} not found", request.PurchaseInvoiceId);
            return Error.NotFound("PurchaseInvoice.NotFound", "Purchase invoice not found.");
        }

        return invoiceDto;
    }
}
