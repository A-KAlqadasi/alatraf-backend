using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Stores.Queries.GetItemUnitQuantityInStoreQuery;

public sealed record GetItemUnitQuantityInStoreQuery(int StoreId, int ItemId, int UnitId) : ICachedQuery<Result<decimal>>
{
	public string CacheKey => $"store:{StoreId}:item:{ItemId}:unit:{UnitId}:qty";
	public string[] Tags => new[] { "store" };
	public TimeSpan Expiration => TimeSpan.FromMinutes(2);
}
