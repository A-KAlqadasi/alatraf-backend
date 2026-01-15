using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Units;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Inventory.Units.Commands.DeleteUnitCommand;

public class DeleteUnitCommandHandler : IRequestHandler<DeleteUnitCommand, Result<Deleted>>
{
    private readonly IAppDbContext _dbContext;

    public DeleteUnitCommandHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Deleted>> Handle(DeleteUnitCommand request, CancellationToken cancellationToken)
    {
        var unit = await _dbContext.Units.SingleOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
        if (unit == null)
            return UnitErrors.UnitNotFound;

        _dbContext.Units.Remove(unit);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Deleted;
    }
}
