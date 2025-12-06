using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Inventory.Purchases.Dtos;
using AlatrafClinic.Application.Features.Inventory.Purchases.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Purchases;
using AlatrafClinic.Domain.Inventory.Stores;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Commands.UpdatePurchaseItem;

public class UpdatePurchaseItemCommandHandler : IRequestHandler<UpdatePurchaseItemCommand, Result<PurchaseInvoiceDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdatePurchaseItemCommandHandler> _logger;

    public UpdatePurchaseItemCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdatePurchaseItemCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<PurchaseInvoiceDto>> Handle(UpdatePurchaseItemCommand request, CancellationToken ct)
    {
        // Load invoice aggregate root
        var invoice = await _unitOfWork.PurchaseInvoices.GetByIdAsync(request.PurchaseInvoiceId, ct);
        if (invoice is null)
        {
            _logger.LogWarning("PurchaseInvoice {Id} not found.", request.PurchaseInvoiceId);
            return Error.NotFound("PurchaseInvoice.NotFound", "Purchase invoice not found.");
        }

        // Load store aggregate (with item units) to find the new StoreItemUnit
        var store = await _unitOfWork.Stores.GetByIdWithItemUnitsAsync(invoice.StoreId, ct);
        if (store is null)
        {
            _logger.LogWarning("Store {StoreId} not found when updating item for invoice {InvoiceId}.", invoice.StoreId, invoice.Id);
            return PurchaseInvoiceErrors.InvalidStore;
        }
        if (store.StoreItemUnits is null)
        {
            _logger.LogWarning("Store {StoreId} has no store item units when updating item for invoice {InvoiceId}.", store.Id, invoice.Id);
            return StoreItemUnitErrors.NotFound;
        }

        var newStoreItemUnit = store.StoreItemUnits.FirstOrDefault(siu => siu.Id == request.NewStoreItemUnitId);
        if (newStoreItemUnit is null)
        {
            _logger.LogWarning("StoreItemUnit {StoreItemUnitId} not found in store {StoreId}.", request.NewStoreItemUnitId, store.Id);
            return StoreItemUnitErrors.NotFound;
        }

        // Delegate to aggregate to perform update (merging logic lives in domain)
        var updateResult = invoice.UpdateItem(request.ExistingStoreItemUnitId, newStoreItemUnit, request.Quantity, request.UnitPrice, request.Notes);
        if (updateResult.IsError)
        {
            _logger.LogWarning("Failed to update purchase item on invoice {InvoiceId}: {Errors}", invoice.Id, string.Join(',', updateResult.Errors));
            return updateResult.Errors;
        }

        await _unitOfWork.PurchaseInvoices.UpdateAsync(invoice, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Updated item on PurchaseInvoice {InvoiceId} (existingStoreItemUnitId={ExistingId}).", invoice.Id, request.ExistingStoreItemUnitId);

        return invoice.ToDto();
    }
}
