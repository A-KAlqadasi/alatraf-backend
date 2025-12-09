
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.InjuryReasons.Queries.GetInjuryReasonById;

public sealed record class GetInjuryReasonByIdQuery(int InjuryReasonId) : ICachedQuery<Result<InjuryDto>>
{
    public string CacheKey => $"injury-reason:{InjuryReasonId}";

    public string[] Tags => new[] { "injury-reason" };

    public TimeSpan Expiration => TimeSpan.FromMinutes(20);
}