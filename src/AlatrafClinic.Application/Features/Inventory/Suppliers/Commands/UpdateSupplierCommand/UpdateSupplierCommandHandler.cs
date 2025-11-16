using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Inventory.Suppliers.Dtos;
using AlatrafClinic.Application.Features.Inventory.Suppliers.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Suppliers;

using MediatR;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Suppliers.Commands.UpdateSupplierCommand;

public class UpdateSupplierCommandHandler : IRequestHandler<UpdateSupplierCommand, Result<SupplierDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger _logger;
    private readonly HybridCache _cache;

    public UpdateSupplierCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateSupplierCommandHandler> logger, HybridCache cache)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _cache = cache;
    }

    public async Task<Result<SupplierDto>> Handle(UpdateSupplierCommand command, CancellationToken ct)
    {
        Supplier? supplier = await _unitOfWork.Suppliers.GetByIdAsync(command.Id, ct);
        if (supplier is null)
        {
            _logger.LogWarning("Supplier with id {SupplierId} not found", command.Id);
            return SupplierErrors.SupplierNotFound;
        }
        var updateResult = supplier.Update(command.SupplierName, command.Phone);
        if (updateResult.IsError)
        {
            _logger.LogWarning("Failed to update supplier with id {SupplierId}: {Errors}", command.Id, updateResult.Errors);
            return updateResult.Errors;
        }
        await _unitOfWork.Suppliers.UpdateAsync(supplier);
        await _unitOfWork.SaveChangesAsync(ct);
        await _cache.RemoveAsync("suppliers_list", ct);
        _logger.LogInformation("Supplier with id {SupplierId} updated successfully.", command.Id);


        return supplier.ToDto();


    }
}