using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Stores.Dtos;
using AlatrafClinic.Domain.Common.Results;
using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Stores.Queries.GetStoreByIdQuery;

public sealed record GetStoreByIdQuery(int StoreId) : ICachedQuery<Result<StoreDto>>
{
	public string CacheKey => $"store:{StoreId}";
	public string[] Tags => new[] { "store" };
	public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}
