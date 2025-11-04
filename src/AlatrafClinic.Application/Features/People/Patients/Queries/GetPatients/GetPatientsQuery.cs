

using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.People.Patients.Dtos;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.People.Patients.Queries.GetPatients;

public sealed record GetPatientsQuery : ICachedQuery<Result<List<PatientDto>>>
{
    public string CacheKey => "patients";
    public string[] Tags => ["patient"];
    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}
