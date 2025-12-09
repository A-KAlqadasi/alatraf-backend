using Microsoft.Extensions.Logging;

using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.People;
using MechanicShop.Application.Common.Errors;
using AlatrafClinic.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.People.Services.UpdatePerson;

public class PersonUpdateService : IPersonUpdateService
{
    private readonly IAppDbContext _context;
    private readonly ILogger<PersonUpdateService> _logger;


  public PersonUpdateService(
    IAppDbContext context,
    ILogger<PersonUpdateService> logger)
    {
        _context = context;
        _logger = logger;
    }

  public async Task<Result<Person>> UpdateAsync(
    int personId,
    string Fullname,
    DateTime Birthdate,
    string Phone,
    string? NationalNo,
    string Address,
    bool Gender,
    CancellationToken ct)
  {
    var person = await _context.People.FirstOrDefaultAsync(p => p.Id == personId, ct);
    if (person is null)
    {
      _logger.LogWarning("Person {personId} not found for update.", personId);
      return ApplicationErrors.PersonNotFound;
    }

    if (!string.IsNullOrWhiteSpace(NationalNo))
    {
        var existing = await _context.People.FirstOrDefaultAsync(p => p.NationalNo == NationalNo.Trim(), ct);

        if (existing is not null && existing.Id != personId)
        {
            _logger.LogWarning("National number already exists for another person: {NationalNo}", NationalNo);
            return PersonErrors.NationalNoExists;
        }
    }

    var isPhoneExists = await _context.People
        .AnyAsync(p => p.Phone == Phone.Trim(), ct);

    if (isPhoneExists && person.Phone != Phone.Trim())
    {
       _logger.LogWarning("Person creation aborted. Phone number already exists: {Phone}", Phone);
       return PersonErrors.PhoneExists; 
    }

    var updateResult = person.Update(
      Fullname.Trim(),
      Birthdate,
      Phone.Trim(),
      NationalNo?.Trim(),
      Address.Trim(),
      Gender);

    if (updateResult.IsError)
    {
      _logger.LogWarning("Update failed for Person {PersonId}: {Error}", personId, updateResult.Errors);
      return updateResult.Errors;
    }

    _logger.LogInformation("Person domain entity prepared to be updated (not persisted yet).");

    return person;
  }
}