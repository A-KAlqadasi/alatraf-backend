using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Hybrid;
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Suppliers;
using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Inventory.Suppliers.Commands.DeleteSupplierCommand;

public class DeleteSupplierCommandHandler : IRequestHandler<DeleteSupplierCommand, Result<Deleted>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<DeleteSupplierCommandHandler> _logger;
    private readonly HybridCache _cache;

    public DeleteSupplierCommandHandler(IAppDbContext dbContext, ILogger<DeleteSupplierCommandHandler> logger, HybridCache cache)
    {
        _dbContext = dbContext;
        _logger = logger;
        _cache = cache;
    }

    public async Task<Result<Deleted>> Handle(DeleteSupplierCommand request, CancellationToken ct)
    {
        var supplier = await _dbContext.Suppliers.SingleOrDefaultAsync(s => s.Id == request.Id, ct);

        if (supplier is null)
        {
            _logger.LogWarning("Supplier with id {SupplierId} not found", request.Id);
            return SupplierErrors.SupplierNotFound;
        }

        _dbContext.Suppliers.Remove(supplier);
        await _dbContext.SaveChangesAsync(ct);

        await _cache.RemoveAsync("suppliers_list", ct);
        _logger.LogInformation("Supplier with id {SupplierId} deleted successfully", request.Id);

        return Result.Deleted;
    }
}
