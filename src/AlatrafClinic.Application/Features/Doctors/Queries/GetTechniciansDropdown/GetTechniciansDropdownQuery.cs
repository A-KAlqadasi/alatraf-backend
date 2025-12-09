
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Doctors.Dtos;

using MediatR;

namespace AlatrafClinic.Application.Features.Doctors.Queries.GetTechniciansDropdown;

public sealed record GetTechniciansDropdownQuery : ICachedQuery<List<TechnicianDto>>
{
    public string CacheKey => "get-doctors";

    public string[] Tags => ["doctor"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(20);
}