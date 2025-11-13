
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Inventory.Items.Dtos;
using AlatrafClinic.Domain.Common.Results;
namespace AlatrafClinic.Application.Features.Inventory.Items.Queries.SearchItemsQuery;

public sealed record SearchItemsQuery(
    string? Keyword = null,
    int? BaseUnitId = null,
    int? UnitId = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    bool? IsActive = null,
    string SortBy = "name",     // "name", "price", "created"
    string SortDir = "asc",     // "asc" or "desc"
    int Page = 1,
    int PageSize = 20
) : ICachedQuery<Result<PagedList<ItemDto>>>
{
    public string CacheKey => $"items:search:{Keyword}:{BaseUnitId}:{UnitId}:{IsActive}:{Page}:{PageSize}:{SortBy}:{SortDir}:{MinPrice}:{MaxPrice}";
    public string[] Tags => ["items"];
    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
