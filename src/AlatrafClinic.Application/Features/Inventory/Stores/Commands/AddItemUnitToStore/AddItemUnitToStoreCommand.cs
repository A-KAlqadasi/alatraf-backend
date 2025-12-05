using AlatrafClinic.Domain.Common.Results;
using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Stores.Commands.AddItemUnitToStore;

public sealed record AddItemUnitToStoreCommand(int StoreId, int ItemId, int UnitId, decimal Quantity)
    : IRequest<Result<Updated>>;
