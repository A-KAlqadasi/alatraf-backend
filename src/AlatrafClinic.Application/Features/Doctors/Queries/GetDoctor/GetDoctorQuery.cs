using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Doctors.Dtos;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.Doctors.Queries.GetDoctor;

public sealed record GetDoctorQuery(int DoctorId) : ICachedQuery<Result<DoctorDto>>
{
    public string CacheKey => $"doctor:{DoctorId}";

    public string[] Tags => ["doctor"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(20);
}