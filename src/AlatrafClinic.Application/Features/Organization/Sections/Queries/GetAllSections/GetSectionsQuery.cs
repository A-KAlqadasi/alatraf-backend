using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Organization.Sections.Dtos;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.Organization.Sections.Queries.GetAllSections;

public sealed record GetSectionsQuery(
    int? DepartmentId = null,
    bool? IsActiveDoctors = null,
    string? SearchTerm = null
) : ICachedQuery<Result<List<SectionDto>>>
{
    public string CacheKey =>
        $"sections:dept={DepartmentId?.ToString() ?? "all"}:search={SearchTerm?.Trim().ToLower() ?? "all"}:active={IsActiveDoctors?.ToString() ?? "all"}";

    public string[] Tags => ["section"];
    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}
