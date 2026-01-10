using AlatrafClinic.Application.Features.Inventory.Units.Dtos;
using AlatrafClinic.Application.Features.Inventory.Units.Mappers;
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Inventory.Units;
using AlatrafClinic.Domain.Common.Results;
using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Units.Commands.CreateUnitCommand;

public class CreateUnitCommandHandler : IRequestHandler<CreateUnitCommand, Result<UnitDto>>
{
    private readonly IAppDbContext _dbContext;

    public CreateUnitCommandHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<UnitDto>> Handle(CreateUnitCommand request, CancellationToken cancellationToken)
    {
        var result = Domain.Inventory.Units.GeneralUnit.Create(request.Name);
        if (result.IsError)
            return result.Errors;

        var unit = result.Value;

        await _dbContext.Units.AddAsync(unit, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return unit.ToDto();
    }
}
