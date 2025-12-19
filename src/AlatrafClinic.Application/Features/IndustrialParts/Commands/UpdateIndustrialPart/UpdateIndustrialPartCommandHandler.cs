using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.RepairCards.IndustrialParts;
using AlatrafClinic.Domain.Inventory.Units;
using AlatrafClinic.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Hybrid;

using MediatR;

using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.IndustrialParts.Commands.UpdateIndustrialPart;

public class UpdateIndustrialPartCommandHandler : IRequestHandler<UpdateIndustrialPartCommand, Result<Updated>>
{
    private readonly ILogger<UpdateIndustrialPartCommandHandler> _logger;
    private readonly IAppDbContext _context;
    private readonly HybridCache _cache;

    public UpdateIndustrialPartCommandHandler(ILogger<UpdateIndustrialPartCommandHandler> logger, IAppDbContext context, HybridCache cache)
    {
        _logger = logger;
        _context = context;
        _cache = cache;
    }
    public async Task<Result<Updated>> Handle(UpdateIndustrialPartCommand command, CancellationToken ct)
    {
        var industrialPart = await _context.IndustrialParts
        .Include(i=> i.IndustrialPartUnits)
        .FirstOrDefaultAsync(i=> i.Id == command.IndustrialPartId, ct);

        if (industrialPart is null)
        {
            _logger.LogError("Industrial part with id {IndustrialPartId} not found", command.IndustrialPartId);
            return IndustrialPartErrors.IndustrialPartNotFound;
        }

        if(industrialPart.Name.Trim() != command.Name.Trim())
        {
            var isExistsName = await _context.IndustrialParts.AnyAsync(i=> i.Name == command.Name.Trim(), ct);
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
        _context.IndustrialParts.Update(industrialPart);
        await _context.SaveChangesAsync(ct);

        List<(int unitId, decimal price)> incomingUnits = new List<(int unitId, decimal price)>();

        foreach (var unit in command.Units)
        {
            var existUnit = await _context.Units.FirstOrDefaultAsync(u=> u.Id == unit.UnitId, ct);
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
        await _context.SaveChangesAsync(ct);
        await _cache.RemoveByTagAsync("industrial-part", ct);
        
        _logger.LogInformation("Industrial part {IndustrialPartName} updated successfully", command.Name);

        return Result.Updated;
    }
}