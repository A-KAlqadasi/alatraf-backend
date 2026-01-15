using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Units.Dtos;
using AlatrafClinic.Application.Features.Inventory.Units.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Units;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Inventory.Units.Queries.GetUnitByIdQuery;

public class GetUnitByIdQueryHandler : IRequestHandler<GetUnitByIdQuery, Result<UnitDto>>
{
    private readonly IAppDbContext _dbContext;

    public GetUnitByIdQueryHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<UnitDto>> Handle(GetUnitByIdQuery request, CancellationToken cancellationToken)
    {
        var unit = await _dbContext.Units.SingleOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
        if (unit == null)
            return UnitErrors.UnitNotFound;

        return unit.ToDto();
    }
}
