using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.People.Persons.Dtos;
using AlatrafClinic.Application.Features.People.Persons.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MechanicShop.Application.Common.Errors;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Persons.Queries.GetPersonById;

public class GetPersonByIdQueryHandler(
    IUnitOfWork unitWork
    )
    : IRequestHandler<GetPersonByIdQuery, Result<PersonDto>>
{
  private readonly IUnitOfWork _unitWork = unitWork;

  public async Task<Result<PersonDto>> Handle(GetPersonByIdQuery query, CancellationToken ct)
  {
    // Load the Person entity
    var person = await _unitWork.Person.GetByIdAsync(query.PersonId, ct);
    if (person is null)
    {
      return ApplicationErrors.PersonNotFound;
    }


    return person.ToDto();
  }
}