using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Inventory.Purchases.Dtos;
using AlatrafClinic.Application.Features.Inventory.Purchases.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Purchases;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Commands.CancelPurchaseInvoice;

public class CancelPurchaseInvoiceCommandHandler : IRequestHandler<CancelPurchaseInvoiceCommand, Result<PurchaseInvoiceDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CancelPurchaseInvoiceCommandHandler> _logger;

    public CancelPurchaseInvoiceCommandHandler(IUnitOfWork unitOfWork, ILogger<CancelPurchaseInvoiceCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<PurchaseInvoiceDto>> Handle(CancelPurchaseInvoiceCommand request, CancellationToken ct)
    {
        var invoice = await _unitOfWork.PurchaseInvoices.GetByIdAsync(request.PurchaseInvoiceId, ct);
        if (invoice is null)
        {
            _logger.LogWarning("PurchaseInvoice {Id} not found.", request.PurchaseInvoiceId);
            return Error.NotFound("PurchaseInvoice.NotFound", "Purchase invoice not found.");
        }

        var cancelResult = invoice.Cancel();
        if (cancelResult.IsError)
        {
            _logger.LogWarning("Failed to cancel PurchaseInvoice {Id}: {Errors}", invoice.Id, string.Join(',', cancelResult.Errors));
            return cancelResult.Errors;
        }

        await _unitOfWork.PurchaseInvoices.UpdateAsync(invoice, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Cancelled PurchaseInvoice {Id}.", invoice.Id);

        return invoice.ToDto();
    }
}
