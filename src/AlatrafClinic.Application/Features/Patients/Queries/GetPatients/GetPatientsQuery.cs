using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Patients.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients.Enums;

namespace AlatrafClinic.Application.Features.Patients.Queries.GetPatients;

public sealed record GetPatientsQuery(
    int Page,
    int PageSize,
    string? SearchTerm = null,
    PatientType? PatientType = null,
    bool? Gender = null,
    DateOnly? BirthdateFrom = null,
    DateOnly? BirthdateTo = null,
    bool? HasNationalNo = null,
    string SortColumn = "fullname",
    string SortDirection = "asc"
) : ICachedQuery<Result<PaginatedList<PatientDto>>>
{
    public string CacheKey =>
        $"patients:p={Page}:ps={PageSize}" +
        $":q={(SearchTerm ?? "-")}" +
        $":type={(PatientType?.ToString() ?? "-")}" +
        $":gender={(Gender?.ToString() ?? "-")}" +
        $":bfrom={(BirthdateFrom?.ToString("yyyyMMdd") ?? "-")}" +
        $":bto={(BirthdateTo?.ToString("yyyyMMdd") ?? "-")}" +
        $":hasNatNo={(HasNationalNo?.ToString() ?? "-")}" +
        $":sort={SortColumn}:{SortDirection}";

    public string[] Tags => ["patient"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}