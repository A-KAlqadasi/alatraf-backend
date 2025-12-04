using AlatrafClinic.Application.Features.Inventory.ExchangeOrders.Dtos;
using AlatrafClinic.Domain.Common.Results;
using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Commands.UpsertExchangeOrderItems;

public sealed record UpsertExchangeOrderItemsCommand(
    int ExchangeOrderId,
    List<UpsertExchangeOrderItem> Items
) : IRequest<Result<ExchangeOrderDto>>;

public sealed record UpsertExchangeOrderItem(int? Id, int StoreItemUnitId, decimal Quantity);
