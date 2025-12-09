
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Doctors.Dtos;

using MediatR;

namespace AlatrafClinic.Application.Features.Doctors.Queries.GetTherapistDropdown;

public sealed record GetTherapistDropdownQuery : ICachedQuery<List<TherapistDto>>
{
    public string CacheKey => "get-doctors";
    public string[] Tags => ["doctor"];
    public TimeSpan Expiration => TimeSpan.FromMinutes(20);
}