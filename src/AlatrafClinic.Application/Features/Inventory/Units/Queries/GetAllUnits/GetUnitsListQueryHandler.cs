using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Units.Dtos;
using AlatrafClinic.Application.Features.Inventory.Units.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Units.Queries.GetUnitsListQuery;

public sealed class GetUnitsListQueryHandler
    : IRequestHandler<GetUnitsListQuery, Result<List<UnitDto>>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<GetUnitsListQueryHandler> _logger;

    public GetUnitsListQueryHandler(IAppDbContext dbContext, ILogger<GetUnitsListQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<List<UnitDto>>> Handle(GetUnitsListQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching all units from database...");

        var units = await _dbContext.Units.ToListAsync(cancellationToken);

        if (units is null || !units.Any())
        {
            _logger.LogWarning("No units found in the system.");
            return new List<UnitDto>(); // قائمة فارغة بدون خطأ
        }


        return units.ToDtoList();
    }
}
