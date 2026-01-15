using MediatR;
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Inventory.Suppliers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Application.Features.Inventory.Suppliers.Commands.CreateSupplierCommand;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Hybrid;
using AlatrafClinic.Application.Features.Inventory.Suppliers.Mappers;
using AlatrafClinic.Application.Features.Inventory.Suppliers.Dtos;
using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Inventory.Suppliers.Commands.CreateSupplierCommand;

public class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, Result<SupplierDto>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<CreateSupplierCommandHandler> _logger;
    private readonly HybridCache _cache;
    public CreateSupplierCommandHandler(IAppDbContext dbContext, ILogger<CreateSupplierCommandHandler> logger, HybridCache cache)
    {
        _dbContext = dbContext;
        _logger = logger;
        _cache = cache;
    }

    public async Task<Result<SupplierDto>> Handle(CreateSupplierCommand request, CancellationToken ct)
    {
        var supplierResult = Supplier.Create(request.SupplierName, request.Phone);

        if (supplierResult.IsError)
        {
            _logger.LogWarning("Failed to create supplier: {Error}", supplierResult.Errors);
            return supplierResult.Errors;
        }

        var supplier = supplierResult.Value;

        await _dbContext.Suppliers.AddAsync(supplier, ct);

        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Supplier {Name} created successfully with ID {Id}.", supplier.SupplierName, supplier.Id);

        return supplierResult.Value.ToDto();
    }

}