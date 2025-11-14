using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Inventory.Units.Dtos;
using AlatrafClinic.Application.Features.Inventory.Units.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Units;
using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Units.Queries.GetUnitByIdQuery;

public class GetUnitByIdQueryHandler : IRequestHandler<GetUnitByIdQuery, Result<UnitDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUnitByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UnitDto>> Handle(GetUnitByIdQuery request, CancellationToken cancellationToken)
    {
        var unit = await _unitOfWork.Units.GetByIdAsync(request.Id, cancellationToken);
        if (unit == null)
            return UnitErrors.UnitNotFound;

        return unit.ToDto();
    }
}
