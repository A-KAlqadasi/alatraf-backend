using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Purchases.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Commands.MarkPurchaseInvoicePaid;

public class MarkPurchaseInvoicePaidCommandHandler : IRequestHandler<MarkPurchaseInvoicePaidCommand, Result<AlatrafClinic.Application.Features.Inventory.Purchases.Dtos.PurchaseInvoiceDto>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<MarkPurchaseInvoicePaidCommandHandler> _logger;

    public MarkPurchaseInvoicePaidCommandHandler(IAppDbContext dbContext, ILogger<MarkPurchaseInvoicePaidCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<AlatrafClinic.Application.Features.Inventory.Purchases.Dtos.PurchaseInvoiceDto>> Handle(MarkPurchaseInvoicePaidCommand request, CancellationToken ct)
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

        var result = invoice.MarkPaid(request.Amount, request.Method, request.Reference);
        if (result.IsError)
        {
            _logger.LogWarning("Failed to mark PurchaseInvoice {Id} paid: {Errors}", invoice.Id, string.Join(',', result.Errors));
            return result.Errors;
        }

        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Marked PurchaseInvoice {Id} as paid.", invoice.Id);

        return invoice.ToDto();
    }
}
