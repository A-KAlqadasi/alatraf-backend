using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Purchases.Dtos;
using AlatrafClinic.Application.Features.Inventory.Purchases.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Purchases;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Commands.SubmitPurchaseInvoice;

public class SubmitPurchaseInvoiceCommandHandler : IRequestHandler<SubmitPurchaseInvoiceCommand, Result<PurchaseInvoiceDto>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<SubmitPurchaseInvoiceCommandHandler> _logger;

    public SubmitPurchaseInvoiceCommandHandler(IAppDbContext dbContext, ILogger<SubmitPurchaseInvoiceCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<PurchaseInvoiceDto>> Handle(SubmitPurchaseInvoiceCommand request, CancellationToken ct)
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

        var postResult = invoice.Post();
        if (postResult.IsError)
        {
            _logger.LogWarning("Failed to post PurchaseInvoice {Id}: {Errors}", invoice.Id, string.Join(',', postResult.Errors));
            return postResult.Errors;
        }

        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Posted PurchaseInvoice {Id}.", invoice.Id);

        return invoice.ToDto();
    }
}
