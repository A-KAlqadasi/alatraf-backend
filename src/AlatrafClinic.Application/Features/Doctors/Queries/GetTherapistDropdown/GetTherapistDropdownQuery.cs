
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Doctors.Dtos;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.Doctors.Queries.GetTherapistDropdown;

public sealed record GetTherapistDropdownQuery(
    int Page,
    int PageSize,
    int? SectionId = null,
    int? RoomId = null,
    string? SearchTerm = null
) : ICachedQuery<Result<PaginatedList<TherapistDto>>>
{
    public string CacheKey =>
        $"therapists-dropdown:p={Page}:ps={PageSize}" +
        $":sec={(SectionId?.ToString() ?? "-")}" +
        $":room={(RoomId?.ToString() ?? "-")}" +
        $":q={(SearchTerm ?? "-")}";

    public string[] Tags => ["doctor"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(20);
}