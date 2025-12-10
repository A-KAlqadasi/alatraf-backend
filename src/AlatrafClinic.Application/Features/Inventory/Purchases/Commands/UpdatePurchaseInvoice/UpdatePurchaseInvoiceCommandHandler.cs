using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Inventory.Purchases.Dtos;
using AlatrafClinic.Application.Features.Inventory.Purchases.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Purchases;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Commands.UpdatePurchaseInvoice;

public class UpdatePurchaseInvoiceCommandHandler : IRequestHandler<UpdatePurchaseInvoiceCommand, Result<PurchaseInvoiceDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdatePurchaseInvoiceCommandHandler> _logger;

    public UpdatePurchaseInvoiceCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdatePurchaseInvoiceCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<PurchaseInvoiceDto>> Handle(UpdatePurchaseInvoiceCommand request, CancellationToken ct)
    {
        var invoice = await _unitOfWork.PurchaseInvoices.GetByIdAsync(request.Id, ct);
        if (invoice is null)
        {
            _logger.LogWarning("PurchaseInvoice {Id} not found.", request.Id);
            return Error.NotFound("PurchaseInvoice.NotFound", "Purchase invoice not found.");
        }

        var supplier = await _unitOfWork.Suppliers.GetByIdAsync(request.SupplierId, ct);
        if (supplier is null)
        {
            _logger.LogWarning("Supplier {SupplierId} not found when updating purchase invoice {InvoiceId}.", request.SupplierId, request.Id);
            return PurchaseInvoiceErrors.InvalidSupplier;
        }

        var store = await _unitOfWork.Stores.GetByIdAsync(request.StoreId, ct);
        if (store is null)
        {
            _logger.LogWarning("Store {StoreId} not found when updating purchase invoice {InvoiceId}.", request.StoreId, request.Id);
            return PurchaseInvoiceErrors.InvalidStore;
        }

        var updateResult = invoice.UpdateHeader(request.Number, request.Date, supplier, store);
        if (updateResult.IsError)
        {
            _logger.LogWarning("Failed to update purchase invoice {Id}: {Errors}", request.Id, string.Join(',', updateResult.Errors));
            return updateResult.Errors;
        }

        await _unitOfWork.PurchaseInvoices.UpdateAsync(invoice, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Updated PurchaseInvoice {Id}.", invoice.Id);

        return invoice.ToDto();
    }
}
