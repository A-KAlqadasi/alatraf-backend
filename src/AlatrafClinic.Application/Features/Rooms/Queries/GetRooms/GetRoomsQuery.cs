using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Rooms.Dtos;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.Rooms.Queries.GetRooms;

public sealed record GetRoomsQuery(
    int Page,
    int PageSize,
    string? SearchTerm = null,
    int? SectionId = null,
    int? DepartmentId = null,
    string SortColumn = "name",
    string SortDirection = "asc"
) : ICachedQuery<Result<PaginatedList<RoomDto>>>
{
    public string CacheKey =>
        $"rooms:p={Page}:ps={PageSize}" +
        $":q={(SearchTerm ?? "-")}" +
        $":sec={(SectionId?.ToString() ?? "-")}" +
        $":dep={(DepartmentId?.ToString() ?? "-")}" +
        $":sort={SortColumn}:{SortDirection}";

    public string[] Tags => ["room"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(20);
}