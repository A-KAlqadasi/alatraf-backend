using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Items.Commands.AddOrUpdateItemUnitCommand;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Items;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class AddOrUpdateItemUnitCommandHandler : IRequestHandler<AddOrUpdateItemUnitCommand, Result<Updated>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<AddOrUpdateItemUnitCommandHandler> _logger;

    public AddOrUpdateItemUnitCommandHandler(IAppDbContext dbContext, ILogger<AddOrUpdateItemUnitCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<Updated>> Handle(AddOrUpdateItemUnitCommand request, CancellationToken cancellationToken)
    {

        var item = await _dbContext.Items.SingleOrDefaultAsync(i => i.Id == request.ItemId, cancellationToken);
        if (item is null)
        {
            _logger.LogWarning("Item with Id {ItemId} not found.", request.ItemId);
            return ItemErrors.NotFound;
        }


        var result = item.AddOrUpdateItemUnit(
            request.UnitId,
            request.Price,
            request.ConversionFactor,
            request.MinPriceToPay,
            request.MaxPriceToPay
        );

        if (result.IsError)
        {
            _logger.LogWarning("Failed to add/update ItemUnit: {Errors}", result.Errors);
            return result.Errors;
        }


        _dbContext.Items.Update(item);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("ItemUnit for Item {ItemId} updated successfully.", request.ItemId);

        return Result.Updated;
    }
}
