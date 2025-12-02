using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.People.Dtos;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.People.Queries.GetPersonById;

public sealed record GetPersonByIdQuery(int PersonId) : ICachedQuery<Result<PersonDto>>
{
    public string CacheKey => $"person_{PersonId}";
    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
    public string[] Tags => ["person"];
}
