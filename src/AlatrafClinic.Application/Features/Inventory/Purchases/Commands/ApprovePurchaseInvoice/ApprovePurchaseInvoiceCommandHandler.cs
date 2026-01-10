using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Purchases.Dtos;
using AlatrafClinic.Application.Features.Inventory.Purchases.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Purchases;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Commands.ApprovePurchaseInvoice;

public class ApprovePurchaseInvoiceCommandHandler : IRequestHandler<ApprovePurchaseInvoiceCommand, Result<PurchaseInvoiceDto>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<ApprovePurchaseInvoiceCommandHandler> _logger;

    public ApprovePurchaseInvoiceCommandHandler(IAppDbContext dbContext, ILogger<ApprovePurchaseInvoiceCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<PurchaseInvoiceDto>> Handle(ApprovePurchaseInvoiceCommand request, CancellationToken ct)
    {
        var invoice = await _dbContext.PurchaseInvoices
            .Include(i => i.Items)
            .ThenInclude(i => i.StoreItemUnit)
            .Include(i => i.Supplier)
            .Include(i => i.Store)
            .SingleOrDefaultAsync(i => i.Id == request.PurchaseInvoiceId, ct);
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

        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Approved/Posted PurchaseInvoice {Id}.", invoice.Id);

        return invoice.ToDto();
    }
}
