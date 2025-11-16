using Microsoft.Extensions.Logging;
using MediatR;

using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.RepairCards.IndustrialParts;
using AlatrafClinic.Domain.Inventory.Units;

namespace AlatrafClinic.Application.Features.IndustrialParts.Commands.UpdateIndustrialPart;

public class UpdateIndustrialPartCommandHandler : IRequestHandler<UpdateIndustrialPartCommand, Result<Updated>>
{
    private readonly ILogger<UpdateIndustrialPartCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateIndustrialPartCommandHandler(ILogger<UpdateIndustrialPartCommandHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<Updated>> Handle(UpdateIndustrialPartCommand command, CancellationToken ct)
    {
        var industrialPart = await _unitOfWork.IndustrialParts.GetByIdAsync(command.IndustrialPartId, ct);

        if (industrialPart is null)
        {
            _logger.LogError("Industrial part with id {IndustrialPartId} not found", command.IndustrialPartId);
            return IndustrialPartErrors.IndustrialPartNotFound;
        }

        if(industrialPart.Name.Trim() != command.Name.Trim())
        {
            var isExistsName = await _unitOfWork.IndustrialParts.IsExistsByName(command.Name, ct);
            if (isExistsName)
            {
                _logger.LogWarning("Industrial part with name {Name} already exists", command.Name);
                return IndustrialPartErrors.NameAlreadyExists;
            }
        }
        var updateResult = industrialPart.Update(command.Name, command.Description);
        if (updateResult.IsError)
        {
            _logger.LogError("Error occurred while updating industrial part {IndustrialPartName}", command.Name);
            return updateResult.Errors;
        }
        
        List<(int unitId, decimal price)> incomingUnits = new List<(int unitId, decimal price)>();

        foreach (var unit in command.Units)
        {
            var existUnit = await _unitOfWork.Units.GetByIdAsync(unit.UnitId, ct);
            if (existUnit is null)
            {
                _logger.LogError("Unit with id {UnitId} not found", unit.UnitId);

                return UnitErrors.UnitNotFound;
            }

            incomingUnits.Add((unit.UnitId, unit.Price));
        }
        
        var result = industrialPart.UpsertUnits(incomingUnits);
        if (result.IsError)
        {
            _logger.LogError("Error occurred while assigning units to industrial part {IndustrialPartName}", command.Name);
            return result.Errors;
        }
        await _unitOfWork.IndustrialParts.UpdateAsync(industrialPart, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        _logger.LogInformation("Industrial part {IndustrialPartName} updated successfully", command.Name);

        return Result.Updated;
    }
}