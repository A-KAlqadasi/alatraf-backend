
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.MedicalPrograms.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.MedicalPrograms.Queries.GetMedicalProgramById;

public sealed record GetMedicalProgramByIdQuery(int MedicalProgramId) : ICachedQuery<Result<MedicalProgramDto>>
{
    public string CacheKey => $"medical-program:{MedicalProgramId}";

    public string[] Tags => ["medical-program"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(20);
}