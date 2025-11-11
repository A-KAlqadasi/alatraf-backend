using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.People.Patients.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients.Enums;

namespace AlatrafClinic.Application.Features.People.Patients.Queries.GetPatients;

public sealed record GetPatientsQuery(
    PatientType? PatientType = null,
    bool? Gender = null,
    string? SearchTerm = null
) : ICachedQuery<Result<List<PatientDto>>>
{
    public string CacheKey =>
        $"patients:type={(PatientType?.ToString() ?? "all")}" +
        $":gender={(Gender?.ToString() ?? "all")}" +
        $":search={(SearchTerm?.ToLower().Trim() ?? "-")}";

    public string[] Tags => ["patient"];
    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}
