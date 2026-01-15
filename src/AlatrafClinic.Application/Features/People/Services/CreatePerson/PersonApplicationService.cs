using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.People;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.People.Services.CreatePerson;

public class PersonCreateService : IPersonCreateService
{
    private readonly IAppDbContext _context;
    private readonly ILogger<PersonCreateService> _logger;

    public PersonCreateService(
        IAppDbContext context,
        ILogger<PersonCreateService> logger)
    {
            _context = context;
            _logger = logger;

    }

    public async Task<Result<Person>> CreateAsync(
        string Fullname,
        DateOnly Birthdate,
        string Phone,
        string? NationalNo,
        int AddressId,
        bool Gender, CancellationToken ct)
    {
        
        if (!string.IsNullOrWhiteSpace(NationalNo))
        {
        var existing = await _context.People
            .AnyAsync(p => p.NationalNo == NationalNo.Trim(), ct);

        if (existing)
        {
            _logger.LogWarning("Person creation aborted. National number already exists: {NationalNo}", NationalNo);
            return PersonErrors.NationalNoExists;
        }
        }

        var isPhoneExists = await _context.People
            .AnyAsync(p => p.Phone == Phone.Trim(), ct);

        if (isPhoneExists)
        {
            _logger.LogWarning("Person creation aborted. Phone number already exists: {Phone}", Phone);
            return PersonErrors.PhoneExists; 
        }
        var isAddressExists =  await _context.Addresses
            .AnyAsync(a => a.Id == AddressId, ct);
        if (!isAddressExists)
        {
            _logger.LogWarning("Person creation aborted. Address not found: {AddressId}", AddressId);
            return PersonErrors.AddressNotFound;
        }

        var createResult = Person.Create(
            Fullname.Trim(),
            Birthdate,
            Phone.Trim(),
            NationalNo?.Trim(),
            AddressId,
            Gender
            );

        if (createResult.IsError)
        {
            return createResult.Errors;
        }


        _logger.LogInformation("Person domain entity  prepered to created (not persisted yet).");


        return createResult.Value;
    }
}
