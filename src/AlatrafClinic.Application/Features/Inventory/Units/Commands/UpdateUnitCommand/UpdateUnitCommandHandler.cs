using AlatrafClinic.Application.Features.Inventory.Units.Dtos;
using AlatrafClinic.Application.Features.Inventory.Units.Mappers;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Inventory.Units;
using AlatrafClinic.Domain.Common.Results;
using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Units.Commands.UpdateUnitCommand;

public class UpdateUnitCommandHandler : IRequestHandler<UpdateUnitCommand, Result<UnitDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUnitCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UnitDto>> Handle(UpdateUnitCommand request, CancellationToken cancellationToken)
    {
        var unit = await _unitOfWork.Units.GetByIdAsync(request.Id, cancellationToken);
        if (unit == null)
            return UnitErrors.UnitNotFound;

        var result = unit.Update(request.Name);
        if (result.IsError)
            return result.Errors;

        await _unitOfWork.Units.UpdateAsync(unit);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return unit.ToDto();
    }
}
