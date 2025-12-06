using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Stores.Commands.AdjustStock;

public sealed record AdjustStockCommand(int StoreId, int ItemId, int UnitId, decimal Quantity, bool Increase)
    : IRequest<Result<Updated>>;
