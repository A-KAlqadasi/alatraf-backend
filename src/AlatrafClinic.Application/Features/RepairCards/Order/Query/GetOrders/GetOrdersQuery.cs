using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetOrders;

public sealed record GetOrdersQuery(
    int? SectionId,
    int? RepairCardId,
    string? Status,
    string? SearchTerm,
    string? SortColumn,
    string? SortDirection,
    int Page = 1,
    int PageSize = 10
) : ICachedQuery<Result<AlatrafClinic.Application.Common.Models.PaginatedList<AlatrafClinic.Application.Features.RepairCards.Dtos.OrderDto>>>
{
    public string CacheKey => "orders_list";
    public string[] Tags => new[] { "order" };
    public TimeSpan Expiration => TimeSpan.FromMinutes(2);
}
