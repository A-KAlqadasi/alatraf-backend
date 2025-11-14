using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Inventory.Units.Dtos;
using AlatrafClinic.Application.Features.Inventory.Units.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Units.Queries.GetUnitsListQuery;

public sealed class GetUnitsListQueryHandler
    : IRequestHandler<GetUnitsListQuery, Result<List<UnitDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetUnitsListQueryHandler> _logger;

    public GetUnitsListQueryHandler(IUnitOfWork unitOfWork, ILogger<GetUnitsListQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<List<UnitDto>>> Handle(GetUnitsListQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching all units from database...");

        var units = await _unitOfWork.Units.GetAllAsync(cancellationToken);

        if (units is null || !units.Any())
        {
            _logger.LogWarning("No units found in the system.");
            return new List<UnitDto>(); // قائمة فارغة بدون خطأ
        }


        return units.ToDtoList();
    }
}
