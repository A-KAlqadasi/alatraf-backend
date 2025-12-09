using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.InjurySides.Queries.GetInjurySideById;

public sealed record GetInjurySideByIdQuery(int InjurySideId)
    : ICachedQuery<Result<InjuryDto>>
{
    public string CacheKey => $"injury-sides:{InjurySideId}";
    public string[] Tags => new[] { "injury-side" };
    public TimeSpan Expiration => TimeSpan.FromMinutes(20);
}