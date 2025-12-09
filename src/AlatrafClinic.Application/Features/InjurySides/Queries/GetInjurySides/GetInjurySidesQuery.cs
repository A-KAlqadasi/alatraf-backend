using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.InjurySides.Queries.GetInjurySides;

public class GetInjurySidesQuery : ICachedQuery<Result<List<InjuryDto>>>
{
    public string CacheKey => "get-injury-sides";

    public string[] Tags => ["injury-side"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(20);
}