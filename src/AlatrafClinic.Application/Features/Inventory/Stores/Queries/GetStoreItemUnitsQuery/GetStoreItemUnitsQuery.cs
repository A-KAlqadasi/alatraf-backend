using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Stores.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Stores.Queries.GetStoreItemUnitsQuery;

public sealed record GetStoreItemUnitsQuery(int StoreId) : ICachedQuery<Result<List<StoreItemUnitDto>>>
{
	public string CacheKey => $"store:{StoreId}:items";
	public string[] Tags => new[] { "store" };
	public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
