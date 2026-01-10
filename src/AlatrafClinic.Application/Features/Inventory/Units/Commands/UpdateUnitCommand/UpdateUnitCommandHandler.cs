using AlatrafClinic.Application.Features.Inventory.Units.Dtos;
using AlatrafClinic.Application.Features.Inventory.Units.Mappers;
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Inventory.Units;
using AlatrafClinic.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Inventory.Units.Commands.UpdateUnitCommand;

public class UpdateUnitCommandHandler : IRequestHandler<UpdateUnitCommand, Result<UnitDto>>
{
    private readonly IAppDbContext _dbContext;

    public UpdateUnitCommandHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<UnitDto>> Handle(UpdateUnitCommand request, CancellationToken cancellationToken)
    {
        var unit = await _dbContext.Units.SingleOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
        if (unit == null)
            return UnitErrors.UnitNotFound;

        var result = unit.Update(request.Name);
        if (result.IsError)
            return result.Errors;

        _dbContext.Units.Update(unit);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return unit.ToDto();
    }
}
