using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Items;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Items.Commands.RemoveItemUnitCommand;


public class RemoveItemUnitCommandHandler : IRequestHandler<RemoveItemUnitCommand, Result<Updated>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<RemoveItemUnitCommandHandler> _logger;

    public RemoveItemUnitCommandHandler(IAppDbContext dbContext, ILogger<RemoveItemUnitCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<Updated>> Handle(RemoveItemUnitCommand request, CancellationToken cancellationToken)
    {

        var item = await _dbContext.Items.SingleOrDefaultAsync(i => i.Id == request.ItemId, cancellationToken);
        if (item == null)
        {
            _logger.LogWarning("Item with Id {ItemId} not found.", request.ItemId);
            return ItemErrors.NotFound;
        }


        var result = item.RemoveItemUnit(request.UnitId);
        if (result.IsError)
        {
            _logger.LogWarning("Failed to remove ItemUnit: {Errors}", result.Errors);
            return result.Errors;
        }

        _dbContext.Items.Update(item);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("ItemUnit with UnitId {UnitId} removed successfully from Item {ItemId}.", request.UnitId, request.ItemId);

        return Result.Updated;
    }
}
