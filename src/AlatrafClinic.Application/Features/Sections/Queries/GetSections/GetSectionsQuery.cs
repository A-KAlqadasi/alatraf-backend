using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Sections.Dtos;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.Sections.Queries.GetSections;

public sealed record GetSectionsQuery(
    int Page,
    int PageSize,
    string? SearchTerm = null,
    int? DepartmentId = null,
    int? DoctorId = null,
    int? RoomId = null,
    string SortColumn = "name",
    string SortDirection = "asc"
) : ICachedQuery<Result<PaginatedList<SectionDto>>>
{
    public string CacheKey =>
        $"sections:p={Page}:ps={PageSize}" +
        $":q={(SearchTerm ?? "-")}" +
        $":dep={(DepartmentId?.ToString() ?? "-")}" +
        $":doc={(DoctorId?.ToString() ?? "-")}" +
        $":room={(RoomId?.ToString() ?? "-")}" +
        $":sort={SortColumn}:{SortDirection}";

    public string[] Tags => ["section"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}
