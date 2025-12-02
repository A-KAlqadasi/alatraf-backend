using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.People.Persons.Dtos;
using AlatrafClinic.Application.Features.People.Persons.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Persons.Queries.GetPeople;

public class GetPeopleQueryHandler(
    IUnitOfWork unitWork
    )
    : IRequestHandler<GetPersonsQuery, Result<List<PersonDto>>>
{
  private readonly IUnitOfWork _unitWork = unitWork;

  public async Task<Result<List<PersonDto>>> Handle(GetPersonsQuery query, CancellationToken ct)
  {
    var persons = await _unitWork.People.GetAllAsync(ct);
    return persons.ToDtos();
  }
}