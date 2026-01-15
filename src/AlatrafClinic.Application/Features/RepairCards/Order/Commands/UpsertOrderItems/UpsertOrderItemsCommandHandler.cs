using Microsoft.Extensions.Logging;

using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Items;
using AlatrafClinic.Domain.RepairCards;

using MediatR;
using AlatrafClinic.Domain.Orders;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.UpsertOrderItems;

public sealed class UpsertOrderItemsCommandHandler : IRequestHandler<UpsertOrderItemsCommand, Result<Updated>>
{
    private readonly ILogger<UpsertOrderItemsCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public UpsertOrderItemsCommandHandler(ILogger<UpsertOrderItemsCommandHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Updated>> Handle(UpsertOrderItemsCommand command, CancellationToken ct)
    {
        var repairCard = await _unitOfWork.RepairCards.GetByIdAsync(command.RepairCardId, ct);
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
            var itemUnit = await _unitOfWork.Items.GetByIdAndUnitIdAsync(item.ItemId, item.UnitId, ct);
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

        await _unitOfWork.RepairCards.UpdateAsync(repairCard, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Upserted {Count} items for Order {OrderId} in RepairCard {RepairCardId}.", command.Items.Count, command.OrderId, command.RepairCardId);

        return Result.Updated;
    }
}
