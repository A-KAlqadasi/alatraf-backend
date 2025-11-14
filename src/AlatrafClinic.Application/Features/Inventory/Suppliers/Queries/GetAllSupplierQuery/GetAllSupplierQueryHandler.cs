using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Inventory.Suppliers.Dtos;
using AlatrafClinic.Application.Features.Inventory.Suppliers.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Suppliers.Queries.GetAllSuppliersQuery;

public sealed class GetAllSuppliersQueryHandler : IRequestHandler<GetAllSuppliersQuery, Result<List<SupplierDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetAllSuppliersQueryHandler> _logger;

    public GetAllSuppliersQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAllSuppliersQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<List<SupplierDto>>> Handle(GetAllSuppliersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching all suppliers from database...");

        var suppliers = await _unitOfWork.Suppliers.GetAllAsync(cancellationToken);

        if (suppliers is null || !suppliers.Any())
        {
            _logger.LogWarning("No suppliers found.");
            return new List<SupplierDto>(); // قائمة فارغة وليس خطأ
        }

        var supplierDtos = suppliers.ToDtoList();
        _logger.LogInformation("Retrieved {Count} suppliers successfully.", supplierDtos.Count);

        return supplierDtos;
    }
}
