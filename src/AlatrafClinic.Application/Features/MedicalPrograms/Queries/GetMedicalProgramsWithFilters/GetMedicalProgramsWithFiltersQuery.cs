using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.MedicalPrograms.Dtos;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.MedicalPrograms.Queries.GetMedicalProgramsWithFilters;

public sealed record GetMedicalProgramsWithFilterQuery(
    int Page,
    int PageSize,
    string? SearchTerm = null,
    int? SectionId = null,
    bool? HasSection = null,
    string SortColumn = "Name",
    string SortDirection = "asc"
) : ICachedQuery<Result<PaginatedList<MedicalProgramDto>>>
{
    public string CacheKey =>
        $"medicalprograms:p={Page}:ps={PageSize}" +
        $":q={(SearchTerm ?? "-")}" +
        $":sec={(SectionId?.ToString() ?? "-")}" +
        $":hasSec={(HasSection?.ToString() ?? "-")}" +
        $":sort={SortColumn}:{SortDirection}";

    public string[] Tags => ["medical-program"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(20);
}
