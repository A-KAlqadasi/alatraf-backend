using Microsoft.Extensions.Logging;

using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.RepairCards.Dtos;
using AlatrafClinic.Application.Features.RepairCards.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Items;
using AlatrafClinic.Domain.RepairCards.Orders;

using MediatR;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.CreateOrderWithItems;

public sealed class CreateOrderWithItemsCommandHandler : IRequestHandler<CreateOrderWithItemsCommand, Result<OrderDto>>
{
    private readonly ILogger<CreateOrderWithItemsCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrderWithItemsCommandHandler(ILogger<CreateOrderWithItemsCommandHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<OrderDto>> Handle(CreateOrderWithItemsCommand command, CancellationToken ct)
    {
        // Validate Section exists
        var section = await _unitOfWork.Sections.GetByIdAsync(command.SectionId, ct);
        if (section is null)
        {
            _logger.LogError("Section with Id {SectionId} not found.", command.SectionId);
            return MechanicShop.Application.Common.Errors.ApplicationErrors.SectionNotFound;
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

        // Create order aggregate via factory
        var orderResult = Order.CreateForRaw(command.SectionId);
        if (orderResult.IsError)
        {
            _logger.LogError("Failed to create Order: {Errors}", orderResult.Errors);
            return orderResult.Errors;
        }

        var order = orderResult.Value;

        // Add items through aggregate method
        var upsertResult = order.UpsertItems(incoming);
        if (upsertResult.IsError)
        {
            _logger.LogError("Failed to add items to Order: {Errors}", upsertResult.Errors);
            return upsertResult.Errors;
        }

        // Persist via Orders repository
        await _unitOfWork.Orders.AddAsync(order, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Order {OrderId} created with {Count} items.", order.Id, incoming.Count);

        return order.ToDto();
    }
}
