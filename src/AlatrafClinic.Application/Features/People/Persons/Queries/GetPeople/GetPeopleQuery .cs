using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.People.Persons.Dtos;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.People.Persons.Queries.GetPeople;

public sealed record GetPersonsQuery : ICachedQuery<Result<List<PersonDto>>>
{
    public string CacheKey => "people";
    public string[] Tags => ["person"];
    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}
