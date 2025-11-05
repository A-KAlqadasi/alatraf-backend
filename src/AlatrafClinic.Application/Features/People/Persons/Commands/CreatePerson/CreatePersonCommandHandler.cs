


using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.People.Persons.Dtos;
using AlatrafClinic.Application.Features.People.Persons.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.People;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.People.Persons.Commands.CreatePerson;

public  class CreatePersonCommandHandler(
    ILogger<CreatePersonCommandHandler> logger,
    IUnitOfWork unitWork
    ) : IRequestHandler<CreatePersonCommand, Result<PersonDto>>
{
  private readonly ILogger<CreatePersonCommandHandler> _logger = logger;
  private readonly IUnitOfWork _unitWork = unitWork;

  public async Task<Result<PersonDto>> Handle(CreatePersonCommand command, CancellationToken cancellationToken)
  {

    if (!string.IsNullOrWhiteSpace(command.NationalNo))
    {
      var existing = await _unitWork.Person
          .GetByNationalNoAsync(command.NationalNo, cancellationToken);

      if (existing is  null)
      {
        _logger.LogWarning("Person creation aborted. National number already exists: {NationalNo}", command.NationalNo);
        return PersonErrors.NationalNoExists;
      }
    }
    var createResult = Person.Create(
        command.Fullname.Trim(),
        command.Birthdate,
        command.Phone?.Trim(),
        command.NationalNo?.Trim(),
        command.Address?.Trim());

    if (createResult.IsError)
    {
      return createResult.Errors;
    }
    var person = createResult.Value;

    await _unitWork.Person.AddAsync(person, cancellationToken);
    await _unitWork.SaveChangesAsync(cancellationToken);

    _logger.LogInformation("Person created successfully with ID: {PersonId}", person.Id);

    return person.ToDto();
  }
}
