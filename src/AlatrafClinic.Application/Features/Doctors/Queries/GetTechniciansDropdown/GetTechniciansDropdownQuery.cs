
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Doctors.Dtos;
using AlatrafClinic.Domain.Common.Results;


namespace AlatrafClinic.Application.Features.Doctors.Queries.GetTechniciansDropdown;

public sealed record GetTechniciansDropdownQuery(
    int Page,
    int PageSize,
    string? SearchTerm = null
) : ICachedQuery<Result<PaginatedList<TechnicianDto>>>
{
    public string CacheKey =>
        $"technicians-dropdown:p={Page}:ps={PageSize}" +
        $":q={(SearchTerm ?? "-")}";

    public string[] Tags => ["doctor"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(20);
}