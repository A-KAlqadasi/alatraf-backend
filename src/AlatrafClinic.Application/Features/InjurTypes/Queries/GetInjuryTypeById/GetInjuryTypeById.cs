
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.InjurTypes.Queries.GetInjuryTypeById;

public sealed record class GetInjuryTypeByIdQuery(int InjuryTypeId) : ICachedQuery<Result<InjuryDto>>
{
    public string CacheKey => $"injury-type:{InjuryTypeId}";

    public string[] Tags => ["injury-type"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(20);
}