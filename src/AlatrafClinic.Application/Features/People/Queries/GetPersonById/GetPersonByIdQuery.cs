using AlatrafClinic.Application.Features.People.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Queries.GetPersonById;

public sealed record GetPersonByIdQuery(int PersonId) : IRequest<Result<PersonDto>>;