using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Units;
using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Units.Commands.DeleteUnitCommand;

public class DeleteUnitCommandHandler : IRequestHandler<DeleteUnitCommand, Result<Deleted>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUnitCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Deleted>> Handle(DeleteUnitCommand request, CancellationToken cancellationToken)
    {
        var unit = await _unitOfWork.Units.GetByIdAsync(request.Id, cancellationToken);
        if (unit == null)
            return UnitErrors.UnitNotFound;

        await _unitOfWork.Units.DeleteAsync(unit);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Deleted;
    }
}
