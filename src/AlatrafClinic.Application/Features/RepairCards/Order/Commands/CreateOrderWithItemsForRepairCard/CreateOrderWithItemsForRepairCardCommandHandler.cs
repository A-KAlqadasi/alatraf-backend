using Microsoft.Extensions.Logging;

using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.RepairCards.Dtos;
using AlatrafClinic.Application.Features.RepairCards.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Items;
using AlatrafClinic.Domain.RepairCards;

using MediatR;
using AlatrafClinic.Application.Common.Errors;
using AlatrafClinic.Domain.Orders;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.CreateOrderWithItemsForRepairCard;

public sealed class CreateOrderWithItemsForRepairCardCommandHandler : IRequestHandler<CreateOrderWithItemsForRepairCardCommand, Result<OrderDto>>
{
    private readonly ILogger<CreateOrderWithItemsForRepairCardCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrderWithItemsForRepairCardCommandHandler(ILogger<CreateOrderWithItemsForRepairCardCommandHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<OrderDto>> Handle(CreateOrderWithItemsForRepairCardCommand command, CancellationToken ct)
    {
        // Load repair card aggregate
        var repairCard = await _unitOfWork.RepairCards.GetByIdAsync(command.RepairCardId, ct);
        if (repairCard is null)
        {
            _logger.LogError("RepairCard with Id {RepairCardId} not found.", command.RepairCardId);
            return RepairCardErrors.RepairCardNotFound;
        }

        // Validate section exists
        var section = await _unitOfWork.Sections.GetByIdAsync(command.SectionId, ct);
        if (section is null)
        {
            _logger.LogError("Section with Id {SectionId} not found.", command.SectionId);
            return ApplicationErrors.SectionNotFound;
        }

        // Validate items and load ItemUnits
        var incoming = new List<(ItemUnit itemUnit, decimal quantity)>();
        foreach (var it in command.Items)
        {
            var itemUnit = await _unitOfWork.Items.GetByIdAndUnitIdAsync(it.ItemId, it.UnitId, ct);
            if (itemUnit is null)
            {
                _logger.LogError("ItemUnit not found (ItemId={ItemId}, UnitId={UnitId}).", it.ItemId, it.UnitId);
                return AlatrafClinic.Domain.Inventory.Items.ItemErrors.ItemUnitNotFound;
            }

            if (it.Quantity <= 0)
            {
                _logger.LogError("Invalid quantity for ItemId {ItemId} UnitId {UnitId}: {Quantity}", it.ItemId, it.UnitId, it.Quantity);
                return OrderErrors.NoItems;
            }

            incoming.Add((itemUnit, it.Quantity));
        }

        // Create order using domain factory tied to repair card
        var orderResult = Order.CreateForRepairCard(command.SectionId, command.RepairCardId);
        if (orderResult.IsError)
        {
            _logger.LogError("Failed to create Order: {Errors}", orderResult.Errors);
            return orderResult.Errors;
        }

        var order = orderResult.Value;

        // Add items through domain behavior on Order
        var upsertResult = order.UpsertItems(incoming);
        if (upsertResult.IsError)
        {
            _logger.LogError("Failed to add items to Order: {Errors}", upsertResult.Errors);
            return upsertResult.Errors;
        }

        // Assign order to repair card via aggregate root method
        var assignResult = repairCard.AssignOrder(order);
        if (assignResult.IsError)
        {
            _logger.LogError("Failed to assign Order to RepairCard {RepairCardId}: {Errors}", command.RepairCardId, assignResult.Errors);
            return assignResult.Errors;
        }

        // Persist aggregate
        await _unitOfWork.RepairCards.UpdateAsync(repairCard, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Order {OrderId} created and assigned to RepairCard {RepairCardId} with {Count} items.", order.Id, command.RepairCardId, incoming.Count);

        return order.ToDto();
    }
}
