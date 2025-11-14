using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Hybrid;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Suppliers;

namespace AlatrafClinic.Application.Features.Inventory.Suppliers.Commands.DeleteSupplierCommand;

public class DeleteSupplierCommandHandler : IRequestHandler<DeleteSupplierCommand, Result<Deleted>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteSupplierCommandHandler> _logger;
    private readonly HybridCache _cache;

    public DeleteSupplierCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteSupplierCommandHandler> logger, HybridCache cache)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _cache = cache;
    }

    public async Task<Result<Deleted>> Handle(DeleteSupplierCommand request, CancellationToken ct)
    {
        var supplier = await _unitOfWork.Suppliers.GetByIdAsync(request.Id, ct);

        if (supplier is null)
        {
            _logger.LogWarning("Supplier with id {SupplierId} not found", request.Id);
            return SupplierErrors.SupplierNotFound;
        }

        await _unitOfWork.Suppliers.DeleteAsync(supplier, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        await _cache.RemoveAsync("suppliers_list", ct);
        _logger.LogInformation("Supplier with id {SupplierId} deleted successfully", request.Id);

        return Result.Deleted;
    }
}
