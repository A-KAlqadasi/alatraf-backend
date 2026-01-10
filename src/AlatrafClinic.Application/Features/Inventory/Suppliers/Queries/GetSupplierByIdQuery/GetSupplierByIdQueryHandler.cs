using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Hybrid;
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Suppliers.Mappers;
using AlatrafClinic.Application.Features.Inventory.Suppliers.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Suppliers;
using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Inventory.Suppliers.Queries.GetSupplierByIdQuery;

public class GetSupplierByIdQueryHandler : IRequestHandler<GetSupplierByIdQuery, Result<SupplierDto>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<GetSupplierByIdQueryHandler> _logger;
    private readonly HybridCache _cache;

    public GetSupplierByIdQueryHandler(IAppDbContext dbContext, ILogger<GetSupplierByIdQueryHandler> logger, HybridCache cache)
    {
        _dbContext = dbContext;
        _logger = logger;
        _cache = cache;
    }

    public async Task<Result<SupplierDto>> Handle(GetSupplierByIdQuery request, CancellationToken ct)
    {
        var supplier = await _dbContext.Suppliers.SingleOrDefaultAsync(s => s.Id == request.Id, ct);
        if (supplier is null)
        {
            _logger.LogWarning("Supplier with id {SupplierId} not found", request.Id);
            return SupplierErrors.SupplierNotFound;
        }

        return supplier.ToDto();
    }
}
