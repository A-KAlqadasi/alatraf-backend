using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.MedicalPrograms.Dtos;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.MedicalPrograms.Queries.GetMedicalPrograms;

public sealed record GetMedicalProgramsQuery(
) : ICachedQuery<Result<List<MedicalProgramDto>>>
{
    public string CacheKey => "medical-programs";

    public string[] Tags => ["medical-program"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(20);
}