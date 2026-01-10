using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Purchases.Dtos;
using AlatrafClinic.Application.Features.Inventory.Purchases.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Purchases;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Commands.CancelPurchaseInvoice;

public class CancelPurchaseInvoiceCommandHandler : IRequestHandler<CancelPurchaseInvoiceCommand, Result<PurchaseInvoiceDto>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<CancelPurchaseInvoiceCommandHandler> _logger;

    public CancelPurchaseInvoiceCommandHandler(IAppDbContext dbContext, ILogger<CancelPurchaseInvoiceCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<PurchaseInvoiceDto>> Handle(CancelPurchaseInvoiceCommand request, CancellationToken ct)
    {
        var invoice = await _dbContext.PurchaseInvoices
            .Include(i => i.Supplier)
            .Include(i => i.Store)
            .SingleOrDefaultAsync(i => i.Id == request.PurchaseInvoiceId, ct);
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

        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Cancelled PurchaseInvoice {Id}.", invoice.Id);

        return invoice.ToDto();
    }
}
