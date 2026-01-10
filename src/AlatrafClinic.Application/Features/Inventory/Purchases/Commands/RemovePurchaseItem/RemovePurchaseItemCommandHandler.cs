using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Purchases.Dtos;
using AlatrafClinic.Application.Features.Inventory.Purchases.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Purchases;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Commands.RemovePurchaseItem;

public class RemovePurchaseItemCommandHandler : IRequestHandler<RemovePurchaseItemCommand, Result<PurchaseInvoiceDto>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<RemovePurchaseItemCommandHandler> _logger;

    public RemovePurchaseItemCommandHandler(IAppDbContext dbContext, ILogger<RemovePurchaseItemCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<PurchaseInvoiceDto>> Handle(RemovePurchaseItemCommand request, CancellationToken ct)
    {
        var invoice = await _dbContext.PurchaseInvoices
            .Include(i => i.Items)
            .Include(i => i.Supplier)
            .Include(i => i.Store)
            .SingleOrDefaultAsync(i => i.Id == request.PurchaseInvoiceId, ct);
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

        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Removed StoreItemUnit {StoreItemUnitId} from PurchaseInvoice {InvoiceId}.", request.StoreItemUnitId, invoice.Id);

        return invoice.ToDto();
    }
}
