using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Purchases.Dtos;
using AlatrafClinic.Application.Features.Inventory.Purchases.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Purchases;
using AlatrafClinic.Domain.Inventory.Stores;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Commands.AddPurchaseItem;

public class AddPurchaseItemCommandHandler : IRequestHandler<AddPurchaseItemCommand, Result<PurchaseInvoiceDto>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<AddPurchaseItemCommandHandler> _logger;

    public AddPurchaseItemCommandHandler(IAppDbContext dbContext, ILogger<AddPurchaseItemCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<PurchaseInvoiceDto>> Handle(AddPurchaseItemCommand request, CancellationToken ct)
    {
        // Load invoice aggregate root
        var invoice = await _dbContext.PurchaseInvoices
            .Include(i => i.Items)
            .Include(i => i.Supplier)
            .Include(i => i.Store)
            .SingleOrDefaultAsync(i => i.Id == request.PurchaseInvoiceId, ct);
        if (invoice is null)
        {
            _logger.LogWarning("PurchaseInvoice {Id} not found.", request.PurchaseInvoiceId);
            return Error.NotFound("PurchaseInvoice.NotFound", "Purchase invoice not found.");
        }

        // Load the store aggregate that should contain the StoreItemUnit
        var store = await _dbContext.Stores
            .Include(s => s.StoreItemUnits)
            .SingleOrDefaultAsync(s => s.Id == invoice.StoreId, ct);
        if (store is null)
        {
            _logger.LogWarning("Store {StoreId} not found when adding item to invoice {InvoiceId}.", invoice.StoreId, invoice.Id);
            return PurchaseInvoiceErrors.InvalidStore;
        }
        if (store.StoreItemUnits is null)
        {
            _logger.LogWarning("Store {StoreId} has no store item units when adding item to invoice {InvoiceId}.", store.Id, invoice.Id);
            return StoreItemUnitErrors.NotFound;
        }

        // Find the StoreItemUnit inside the store aggregate
        var storeItemUnit = store.StoreItemUnits.FirstOrDefault(siu => siu.Id == request.StoreItemUnitId);
        if (storeItemUnit is null)
        {
            _logger.LogWarning("StoreItemUnit {StoreItemUnitId} not found in store {StoreId}.", request.StoreItemUnitId, store.Id);
            return StoreItemUnitErrors.NotFound;
        }

        // Delegate to aggregate root to add/merge the purchase line
        var addResult = invoice.AddItem(storeItemUnit, request.Quantity, request.UnitPrice, request.Notes);
        if (addResult.IsError)
        {
            _logger.LogWarning("Failed to add purchase item to invoice {InvoiceId}: {Errors}", invoice.Id, string.Join(',', addResult.Errors));
            return addResult.Errors;
        }

        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Added item {StoreItemUnitId} to PurchaseInvoice {InvoiceId}.", request.StoreItemUnitId, invoice.Id);

        return invoice.ToDto();
    }
}
