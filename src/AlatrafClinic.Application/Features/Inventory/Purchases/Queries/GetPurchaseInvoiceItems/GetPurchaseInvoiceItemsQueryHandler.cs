using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Purchases.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Queries.GetPurchaseInvoiceItems;

public class GetPurchaseInvoiceItemsQueryHandler : IRequestHandler<GetPurchaseInvoiceItemsQuery, Result<List<PurchaseItemDto>>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<GetPurchaseInvoiceItemsQueryHandler> _logger;

    public GetPurchaseInvoiceItemsQueryHandler(IAppDbContext dbContext, ILogger<GetPurchaseInvoiceItemsQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<List<AlatrafClinic.Application.Features.Inventory.Purchases.Dtos.PurchaseItemDto>>> Handle(GetPurchaseInvoiceItemsQuery request, CancellationToken ct)
    {
        var items = await _dbContext.PurchaseItems
            .AsNoTracking()
            .Where(i => i.PurchaseInvoiceId == request.PurchaseInvoiceId)
            .Select(i => new PurchaseItemDto
            {
                Id = i.Id,
                StoreItemUnitId = i.StoreItemUnitId,
                ItemId = i.StoreItemUnit.ItemUnit.ItemId,
                ItemName = i.StoreItemUnit.ItemUnit.Item.Name,
                UnitId = i.StoreItemUnit.ItemUnit.UnitId,
                UnitName = i.StoreItemUnit.ItemUnit.Unit.Name,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                Total = i.Total,
                Notes = i.Notes
            })
            .ToListAsync(ct);

        if (items.Count == 0)
        {
            _logger.LogInformation("No items found for PurchaseInvoice {Id}", request.PurchaseInvoiceId);
        }

        return items;
    }
}
