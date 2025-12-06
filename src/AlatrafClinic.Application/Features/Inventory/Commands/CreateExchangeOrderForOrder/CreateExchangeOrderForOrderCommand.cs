using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Commands.CreateExchangeOrderForOrder;

public sealed record CreateExchangeOrderForOrderCommand(
    int OrderId,
    int StoreId,
    string Number,
    List<CreateExchangeOrderItem> Items,
    string? Notes
) : IRequest<Result<AlatrafClinic.Application.Features.Inventory.ExchangeOrders.Dtos.ExchangeOrderDto>>;

public sealed record CreateExchangeOrderItem(int StoreItemUnitId, decimal Quantity);
