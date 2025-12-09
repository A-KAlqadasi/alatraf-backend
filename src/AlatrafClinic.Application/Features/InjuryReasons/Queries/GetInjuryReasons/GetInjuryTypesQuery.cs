using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.InjuryReasons.Queries.GetInjuryReasons;

public class GetInjuryReasonsQuery : ICachedQuery<Result<List<InjuryDto>>>
{
    public string CacheKey => "get-injury-reasons";

    public string[] Tags => ["injury-reason"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(20);
}