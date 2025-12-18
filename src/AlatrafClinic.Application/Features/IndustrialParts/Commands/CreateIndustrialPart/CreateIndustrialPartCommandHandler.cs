using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.IndustrialParts.Dtos;
using AlatrafClinic.Application.Features.IndustrialParts.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Units;
using AlatrafClinic.Domain.RepairCards.IndustrialParts;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.IndustrialParts.Commands.CreateIndustrialPart;

public class CreateIndustrialPartCommandHandler : IRequestHandler<CreateIndustrialPartCommand, Result<IndustrialPartDto>>
{
    private readonly ILogger<CreateIndustrialPartCommandHandler> _logger;
    private readonly IAppDbContext _context;
    private readonly HybridCache _cache;

    public CreateIndustrialPartCommandHandler(ILogger<CreateIndustrialPartCommandHandler> logger, IAppDbContext context, HybridCache cache)
    {
        _logger = logger;
        _context = context;
        _cache = cache;
    }
    public async Task<Result<IndustrialPartDto>> Handle(CreateIndustrialPartCommand command, CancellationToken ct)
    {
        var isExistsName = await _context.IndustrialParts.AnyAsync(i=> i.Name == command.Name.Trim(), ct);

        if (isExistsName)
        {
            _logger.LogWarning("Industrial part with name {Name} already exists", command.Name);
            return IndustrialPartErrors.NameAlreadyExists;
        }
        var createResult = IndustrialPart.Create(command.Name, command.Description);
        if (createResult.IsError)
        {
            _logger.LogError("Error occurred while creating industrial part {IndustrialPartName}", command.Name);
            return createResult.Errors;
        }
        IndustrialPart industrialPart = createResult.Value;

        await _context.IndustrialParts.AddAsync(industrialPart, ct);
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
        
        var UpsertResult = industrialPart.UpsertUnits(incomingUnits);
        if (UpsertResult.IsError)
        {
            _logger.LogError("Error occurred while assigning units to industrial part {IndustrialPartName}", command.Name);
            return UpsertResult.Errors;
        }
        
        await _context.SaveChangesAsync(ct);
        await _cache.RemoveByTagAsync("industrial-part");

        _logger.LogInformation("Industrial part {IndustrialPartName} created successfully", command.Name);

        return industrialPart.ToDto();
    }
}