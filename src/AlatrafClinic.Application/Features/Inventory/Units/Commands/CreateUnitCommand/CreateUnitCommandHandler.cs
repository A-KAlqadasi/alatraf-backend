using AlatrafClinic.Application.Features.Inventory.Units.Dtos;
using AlatrafClinic.Application.Features.Inventory.Units.Mappers;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Inventory.Units;
using AlatrafClinic.Domain.Common.Results;
using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Units.Commands.CreateUnitCommand;

public class CreateUnitCommandHandler : IRequestHandler<CreateUnitCommand, Result<UnitDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateUnitCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UnitDto>> Handle(CreateUnitCommand request, CancellationToken cancellationToken)
    {
        var result = Domain.Inventory.Units.Unit.Create(request.Name);
        if (result.IsError)
            return result.Errors;

        var unit = result.Value;

        await _unitOfWork.Units.AddAsync(unit);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return unit.ToDto();
    }
}
