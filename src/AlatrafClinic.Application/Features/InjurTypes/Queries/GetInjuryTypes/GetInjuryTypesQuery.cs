
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.InjurTypes.Queries.GetInjuryTypes;

public class GetInjuryTypesQuery : ICachedQuery<Result<List<InjuryDto>>>
{
    public string CacheKey => "get-injury-types";

    public string[] Tags => ["injury-type"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(20);
}