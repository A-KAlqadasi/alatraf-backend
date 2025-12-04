using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Inventory.Purchases.Dtos;
using AlatrafClinic.Application.Features.Inventory.Purchases.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Purchases;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Commands.ApprovePurchaseInvoice;

public class ApprovePurchaseInvoiceCommandHandler : IRequestHandler<ApprovePurchaseInvoiceCommand, Result<PurchaseInvoiceDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ApprovePurchaseInvoiceCommandHandler> _logger;

    public ApprovePurchaseInvoiceCommandHandler(IUnitOfWork unitOfWork, ILogger<ApprovePurchaseInvoiceCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<PurchaseInvoiceDto>> Handle(ApprovePurchaseInvoiceCommand request, CancellationToken ct)
    {
        var invoice = await _unitOfWork.PurchaseInvoices.GetByIdAsync(request.PurchaseInvoiceId, ct);
        if (invoice is null)
        {
            _logger.LogWarning("PurchaseInvoice {Id} not found.", request.PurchaseInvoiceId);
            return Error.NotFound("PurchaseInvoice.NotFound", "Purchase invoice not found.");
        }

        // Use the domain's Post() method to apply inventory movements and set status.
        var result = invoice.Post();
        if (result.IsError)
        {
            _logger.LogWarning("Failed to approve/post PurchaseInvoice {Id}: {Errors}", invoice.Id, string.Join(',', result.Errors));
            return result.Errors;
        }

        await _unitOfWork.PurchaseInvoices.UpdateAsync(invoice, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Approved/Posted PurchaseInvoice {Id}.", invoice.Id);

        return invoice.ToDto();
    }
}
