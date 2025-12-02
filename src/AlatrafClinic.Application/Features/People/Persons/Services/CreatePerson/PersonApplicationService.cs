
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.People;

using FluentValidation;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.People.Persons.Services;

public class PersonCreateService : IPersonCreateService
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly ILogger<PersonCreateService> _logger;
  private readonly IValidator<PersonInput> _validator; // <- it will be injected automatically

  public PersonCreateService(
      IUnitOfWork unitOfWork,
      ILogger<PersonCreateService> logger,
        IValidator<PersonInput> validator)
  {
    _unitOfWork = unitOfWork;
    _logger = logger;
    _validator = validator;

  }

  public async Task<Result<Person>> CreateAsync(PersonInput person, CancellationToken cancellationToken)
  {
    
    if (!string.IsNullOrWhiteSpace(person.NationalNo))
    {
      var existing = await _unitOfWork.People
          .GetByNationalNoAsync(person.NationalNo.Trim(), cancellationToken);

      if (existing is not null)
      {
        _logger.LogWarning("Person creation aborted. National number already exists: {NationalNo}", person.NationalNo);
        return PersonErrors.NationalNoExists;
      }
    }

    var createResult = Person.Create(
       person.Fullname.Trim(),
            person.Birthdate,
           person.Phone.Trim(),
           person.NationalNo?.Trim(),
           person.Address.Trim(),

person.Gender
           );

    if (createResult.IsError)
      return createResult.Errors;


    _logger.LogInformation("Person domain entity  prepered to created (not persisted yet).");


    return createResult.Value;
  }
}
