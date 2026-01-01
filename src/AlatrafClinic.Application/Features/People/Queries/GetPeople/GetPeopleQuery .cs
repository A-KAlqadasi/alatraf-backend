using AlatrafClinic.Application.Features.People.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Queries.GetPeople;

public sealed record GetPersonsQuery : IRequest<Result<List<PersonDto>>>;