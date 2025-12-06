using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Stores.Commands.RemoveItemUnitFromStore;

public sealed record RemoveItemUnitFromStoreCommand(int StoreId, int ItemId, int UnitId) : IRequest<Result<Deleted>>;
