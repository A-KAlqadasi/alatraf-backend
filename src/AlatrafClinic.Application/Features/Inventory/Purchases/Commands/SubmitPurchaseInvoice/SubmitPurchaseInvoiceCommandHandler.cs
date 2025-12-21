using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Inventory.Purchases.Dtos;
using AlatrafClinic.Application.Features.Inventory.Purchases.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Purchases;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Commands.SubmitPurchaseInvoice;

public class SubmitPurchaseInvoiceCommandHandler : IRequestHandler<SubmitPurchaseInvoiceCommand, Result<PurchaseInvoiceDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SubmitPurchaseInvoiceCommandHandler> _logger;

    public SubmitPurchaseInvoiceCommandHandler(IUnitOfWork unitOfWork, ILogger<SubmitPurchaseInvoiceCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<PurchaseInvoiceDto>> Handle(SubmitPurchaseInvoiceCommand request, CancellationToken ct)
    {
        var invoice = await _unitOfWork.PurchaseInvoices
         .GetByIdWithItemsAsync(request.PurchaseInvoiceId, ct);
        if (invoice is null)
        {
            _logger.LogWarning("PurchaseInvoice {Id} not found.", request.PurchaseInvoiceId);
            return Error.NotFound("PurchaseInvoice.NotFound", "Purchase invoice not found.");
        }

        var postResult = invoice.Post();
        if (postResult.IsError)
        {
            _logger.LogWarning("Failed to post PurchaseInvoice {Id}: {Errors}", invoice.Id, string.Join(',', postResult.Errors));
            return postResult.Errors;
        }

        await _unitOfWork.PurchaseInvoices.UpdateAsync(invoice, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Posted PurchaseInvoice {Id}.", invoice.Id);

        return invoice.ToDto();
    }
}
