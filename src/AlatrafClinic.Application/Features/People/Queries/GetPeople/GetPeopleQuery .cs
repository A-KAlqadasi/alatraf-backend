using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.People.Dtos;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.People.Queries.GetPeople;

public sealed record GetPersonsQuery : ICachedQuery<Result<List<PersonDto>>>
{
    public string CacheKey => "people";
    public string[] Tags => ["person"];
    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}
