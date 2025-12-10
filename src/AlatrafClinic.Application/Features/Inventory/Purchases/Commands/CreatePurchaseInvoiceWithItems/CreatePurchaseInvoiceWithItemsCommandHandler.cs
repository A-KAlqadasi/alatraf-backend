using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Inventory.Purchases.Dtos;
using AlatrafClinic.Application.Features.Inventory.Purchases.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Purchases;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Commands.CreatePurchaseInvoiceWithItems;

public class CreatePurchaseInvoiceWithItemsCommandHandler : IRequestHandler<CreatePurchaseInvoiceWithItemsCommand, Result<PurchaseInvoiceDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreatePurchaseInvoiceWithItemsCommandHandler> _logger;

    public CreatePurchaseInvoiceWithItemsCommandHandler(IUnitOfWork unitOfWork, ILogger<CreatePurchaseInvoiceWithItemsCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<PurchaseInvoiceDto>> Handle(CreatePurchaseInvoiceWithItemsCommand request, CancellationToken ct)
    {
        var supplier = await _unitOfWork.Suppliers.GetByIdAsync(request.SupplierId, ct);
        if (supplier is null)
        {
            _logger.LogWarning("Supplier {SupplierId} not found when creating purchase invoice.", request.SupplierId);
            return PurchaseInvoiceErrors.InvalidSupplier;
        }

        var store = await _unitOfWork.Stores.GetByIdWithItemUnitsAsync(request.StoreId, ct);
        if (store is null)
        {
            _logger.LogWarning("Store {StoreId} not found when creating purchase invoice.", request.StoreId);
            return PurchaseInvoiceErrors.InvalidStore;
        }
        if (store.StoreItemUnits is null)
        {
            _logger.LogWarning("Store {StoreId} has no store item units when creating purchase invoice {Number}.", store.Id, request.Number);
            return PurchaseInvoiceErrors.InvalidStore;
        }

        var createResult = PurchaseInvoice.Create(request.Number, request.Date, supplier, store);
        if (createResult.IsError)
        {
            _logger.LogWarning("Failed to create purchase invoice: {Errors}", string.Join(',', createResult.Errors));
            return createResult.Errors;
        }

        var invoice = createResult.Value;

        // Add items
        foreach (var it in request.Items ?? Enumerable.Empty<CreatePurchaseInvoiceWithItemsCommand.CreatePurchaseInvoiceItem>())
        {
            var storeItem = store.StoreItemUnits.FirstOrDefault(s => s.Id == it.StoreItemUnitId);
            if (storeItem is null)
            {
                _logger.LogWarning("StoreItemUnit {StoreItemUnitId} not found in Store {StoreId} when creating invoice.", it.StoreItemUnitId, store.Id);
                return PurchaseItemErrors.InvalidItem;
            }

            var addResult = invoice.AddItem(storeItem, it.Quantity, it.UnitPrice, it.Notes);
            if (addResult.IsError)
            {
                _logger.LogWarning("Failed to add item {StoreItemUnitId} to invoice: {Errors}", it.StoreItemUnitId, string.Join(',', addResult.Errors));
                return addResult.Errors;
            }
        }

        await _unitOfWork.PurchaseInvoices.AddAsync(invoice, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Created PurchaseInvoice {Id} with {Lines} items for supplier {SupplierId}", invoice.Id, invoice.Items.Count, supplier.Id);

        return invoice.ToDto();
    }
}
