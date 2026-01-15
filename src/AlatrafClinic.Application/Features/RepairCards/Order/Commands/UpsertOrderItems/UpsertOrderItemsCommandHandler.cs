using Microsoft.Extensions.Logging;

using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Items;
using AlatrafClinic.Domain.RepairCards;

using MediatR;

using AlatrafClinic.Domain.Orders;

using Microsoft.EntityFrameworkCore;


namespace AlatrafClinic.Application.Features.RepairCards.Commands.UpsertOrderItems;

public sealed class UpsertOrderItemsCommandHandler : IRequestHandler<UpsertOrderItemsCommand, Result<Updated>>
{
    private readonly ILogger<UpsertOrderItemsCommandHandler> _logger;
    private readonly IAppDbContext _dbContext;

    public UpsertOrderItemsCommandHandler(ILogger<UpsertOrderItemsCommandHandler> logger, IAppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<Result<Updated>> Handle(UpsertOrderItemsCommand command, CancellationToken ct)
    {
        var repairCard = await _dbContext.RepairCards
            .Include(rc => rc.Orders)
                .ThenInclude(o => o.OrderItems)
            .SingleOrDefaultAsync(rc => rc.Id == command.RepairCardId, ct);
        if (repairCard is null)
        {
            _logger.LogError("RepairCard with Id {RepairCardId} not found.", command.RepairCardId);
            return RepairCardErrors.InvalidDiagnosisId;
        }

        if (command.Items is null || command.Items.Count == 0)
        {
            return OrderErrors.NoItems;
        }

        var incoming = new List<(ItemUnit itemUnit, decimal quantity)>();

        foreach (var item in command.Items)
        {
            var itemUnit = await _dbContext.ItemUnits
                .Include(iu => iu.Item)
                .Include(iu => iu.Unit)
                .SingleOrDefaultAsync(iu => iu.ItemId == item.ItemId && iu.UnitId == item.UnitId, ct);
            if (itemUnit is null)
            {
                _logger.LogError("ItemUnit not found (ItemId={ItemId}, UnitId={UnitId}).", item.ItemId, item.UnitId);
                return AlatrafClinic.Domain.Inventory.Items.ItemErrors.ItemUnitNotFound;
            }

            if (item.Quantity <= 0)
            {
                _logger.LogError("Invalid quantity for ItemId {ItemId} UnitId {UnitId}: {Quantity}", item.ItemId, item.UnitId, item.Quantity);
                return OrderErrors.NoItems; // reuse a generic error; could add a specific one
            }

            incoming.Add((itemUnit, item.Quantity));
        }

        var result = repairCard.UpsertOrderItems(command.OrderId, incoming);
        if (result.IsError)
        {
            _logger.LogError("Failed to upsert items for Order {OrderId} in RepairCard {RepairCardId}: {Errors}", command.OrderId, command.RepairCardId, result.Errors);
            return result.Errors;
        }

        _dbContext.RepairCards.Update(repairCard);
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Upserted {Count} items for Order {OrderId} in RepairCard {RepairCardId}.", command.Items.Count, command.OrderId, command.RepairCardId);

        return Result.Updated;
    }
}
