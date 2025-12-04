using MediatR;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Inventory.Suppliers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Application.Features.Inventory.Suppliers.Commands.CreateSupplierCommand;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Hybrid;
using AlatrafClinic.Application.Features.Inventory.Suppliers.Mappers;
using AlatrafClinic.Application.Features.Inventory.Suppliers.Dtos;

namespace AlatrafClinic.Application.Features.Inventory.Suppliers.Commands.CreateSupplierCommand;

public class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, Result<SupplierDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger _logger;
    private readonly HybridCache _cache;
    public CreateSupplierCommandHandler(IUnitOfWork unitOfWork, ILogger logger, HybridCache cache)
    {
        _unitOfWork = unitOfWork;
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

        await _unitOfWork.Suppliers.AddAsync(supplier, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Supplier {Name} created successfully with ID {Id}.", supplier.SupplierName, supplier.Id);

        return supplierResult.Value.ToDto();
    }

}