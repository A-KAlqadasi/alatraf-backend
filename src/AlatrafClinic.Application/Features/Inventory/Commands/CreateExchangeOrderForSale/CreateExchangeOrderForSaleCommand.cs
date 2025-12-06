using AlatrafClinic.Application.Features.Inventory.ExchangeOrders.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Commands.CreateExchangeOrderForSale;

public sealed record CreateExchangeOrderForSaleCommand(
    int SaleId,
    int StoreId,
    string Number,
    List<CreateExchangeOrderItem> Items,
    string? Notes
) : IRequest<Result<ExchangeOrderDto>>;

public sealed record CreateExchangeOrderItem(int StoreItemUnitId, decimal Quantity);
