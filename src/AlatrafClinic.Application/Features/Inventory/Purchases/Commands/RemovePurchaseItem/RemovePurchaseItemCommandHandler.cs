using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Inventory.Purchases.Dtos;
using AlatrafClinic.Application.Features.Inventory.Purchases.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Purchases;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Commands.RemovePurchaseItem;

public class RemovePurchaseItemCommandHandler : IRequestHandler<RemovePurchaseItemCommand, Result<PurchaseInvoiceDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RemovePurchaseItemCommandHandler> _logger;

    public RemovePurchaseItemCommandHandler(IUnitOfWork unitOfWork, ILogger<RemovePurchaseItemCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<PurchaseInvoiceDto>> Handle(RemovePurchaseItemCommand request, CancellationToken ct)
    {
        var invoice = await _unitOfWork.PurchaseInvoices.GetByIdAsync(request.PurchaseInvoiceId, ct);
        if (invoice is null)
        {
            _logger.LogWarning("PurchaseInvoice {Id} not found.", request.PurchaseInvoiceId);
            return Error.NotFound("PurchaseInvoice.NotFound", "Purchase invoice not found.");
        }

        var removeResult = invoice.RemoveItem(request.StoreItemUnitId);
        if (removeResult.IsError)
        {
            _logger.LogWarning("Failed to remove item {StoreItemUnitId} from invoice {InvoiceId}: {Errors}", request.StoreItemUnitId, invoice.Id, string.Join(',', removeResult.Errors));
            return removeResult.Errors;
        }

        await _unitOfWork.PurchaseInvoices.UpdateAsync(invoice, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Removed StoreItemUnit {StoreItemUnitId} from PurchaseInvoice {InvoiceId}.", request.StoreItemUnitId, invoice.Id);

        return invoice.ToDto();
    }
}
