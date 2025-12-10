using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.MedicalPrograms.Dtos;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.MedicalPrograms.Queries.GetMedicalPrograms;

public sealed record GetMedicalProgramsQuery(
    int Page,
    int PageSize,
    string? SearchTerm = null
) : ICachedQuery<Result<PaginatedList<MedicalProgramDto>>>
{
    public string CacheKey =>
        $"medical-programs-dropdown:p={Page}:ps={PageSize}" +
        $":q={(SearchTerm ?? "-")}";

    public string[] Tags => ["medical-program"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(20);
}