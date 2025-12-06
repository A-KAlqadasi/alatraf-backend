using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Inventory.Purchases.Dtos;
using AlatrafClinic.Application.Features.Inventory.Purchases.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Purchases;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Commands.CreatePurchaseInvoice;

public class CreatePurchaseInvoiceCommandHandler : IRequestHandler<CreatePurchaseInvoiceCommand, Result<PurchaseInvoiceDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreatePurchaseInvoiceCommandHandler> _logger;

    public CreatePurchaseInvoiceCommandHandler(IUnitOfWork unitOfWork, ILogger<CreatePurchaseInvoiceCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<PurchaseInvoiceDto>> Handle(CreatePurchaseInvoiceCommand request, CancellationToken ct)
    {
        // Load required aggregate roots
        var supplier = await _unitOfWork.Suppliers.GetByIdAsync(request.SupplierId, ct);
        if (supplier is null)
        {
            _logger.LogWarning("Supplier {SupplierId} not found when creating purchase invoice.", request.SupplierId);
            return PurchaseInvoiceErrors.InvalidSupplier;
        }

        var store = await _unitOfWork.Stores.GetByIdAsync(request.StoreId, ct);
        if (store is null)
        {
            _logger.LogWarning("Store {StoreId} not found when creating purchase invoice.", request.StoreId);
            return PurchaseInvoiceErrors.InvalidStore;
        }

        var createResult = PurchaseInvoice.Create(request.Number, request.Date, supplier, store);
        if (createResult.IsError)
        {
            _logger.LogWarning("Failed to create purchase invoice: {Errors}", string.Join(',', createResult.Errors));
            return createResult.Errors;
        }

        var invoice = createResult.Value;

        await _unitOfWork.PurchaseInvoices.AddAsync(invoice, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Created PurchaseInvoice {Id} for supplier {SupplierId}", invoice.Id, supplier.Id);

        return invoice.ToDto();
    }
}
