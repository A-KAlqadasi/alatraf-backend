using AlatrafClinic.Domain.Common.Results;
using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Stores.Commands.TransferStock;

public sealed record TransferStockCommand(int SourceStoreId, int DestinationStoreId, int ItemId, int UnitId, decimal Quantity)
    : IRequest<Result<Updated>>;
