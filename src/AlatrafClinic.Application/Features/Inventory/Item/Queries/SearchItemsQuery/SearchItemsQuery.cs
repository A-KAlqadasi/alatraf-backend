
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Inventory.Items.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;
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
) : IRequest<Result<PagedList<ItemDto>>>;
