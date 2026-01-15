using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Stores.Queries.GetItemUnitQuantityInStoreQuery;

public sealed record GetItemUnitQuantityInStoreQuery(int StoreId, int ItemId, int UnitId) : IRequest<Result<decimal>>;
