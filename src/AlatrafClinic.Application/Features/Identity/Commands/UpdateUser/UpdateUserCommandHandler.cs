using Microsoft.Extensions.Logging;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.People;

using MediatR;
using AlatrafClinic.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using AlatrafClinic.Application.Common.Errors;

namespace AlatrafClinic.Application.Features.Identity.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<Updated>>
{
    private readonly ILogger<UpdateUserCommandHandler> _logger;
    private readonly IIdentityService _identityService;
    private readonly IAppDbContext _context;

    public UpdateUserCommandHandler(ILogger<UpdateUserCommandHandler> logger, IIdentityService identityService, IAppDbContext context)
    {
        _logger = logger;
        _identityService = identityService;
        _context = context;
    }

    public async Task<Result<Updated>> Handle(UpdateUserCommand command, CancellationToken ct)
    {
        var userResult = await _identityService.GetUserByIdAsync(command.UserId.ToString());

        if(userResult is null)
        {
            _logger.LogError("User with {id}, is not exists", command.UserId);
            return ApplicationErrors.UserIsNotFound;
        }
        var user = userResult.Value;
        var person = await _context.People.FirstOrDefaultAsync(p => p.Id == user.PersonId, ct);
        if(person is null)
        {
            _logger.LogError("Person {personId}, is not found for user {userId}", user.PersonId, user.UserId);
            return PersonErrors.NotFound;
        }
        
        bool isNationalNoExist = await _context.People
            .AnyAsync(p => p.NationalNo == command.NationalNo.Trim(), ct);

        if (isNationalNoExist && person.NationalNo?.Trim() != command.NationalNo.Trim())
        {
            _logger.LogWarning("National number already exists: {NationalNo}", command.NationalNo);
            return PersonErrors.NationalNoExists;
        }

        bool isPhoneNumberExist = await _context.People
            .AnyAsync(p => p.Phone == command.Phone.Trim(), ct);

        if (isPhoneNumberExist && person.Phone.Trim() != command.Phone.Trim())
        {
            _logger.LogWarning("Phone number already exists: {Phone}", command.Phone);
            return PersonErrors.PhoneExists;
        }
        bool isNameExist = await _context.People
            .AnyAsync(p => p.FullName == command.Fullname.Trim(), ct);

        if (isNameExist && person.FullName.Trim() != command.Fullname.Trim())
        {
            _logger.LogWarning("Name {fullName} already exist", command.Fullname.Trim());
            return PersonErrors.NameIsExist;
        }

        var updateResult = person.Update(command.Fullname, command.Birthdate, command.Phone, command.NationalNo, command.Address, command.Gender);

        if (updateResult.IsError)
        {
            return updateResult.Errors;
        }

        _context.People.Update(person);
        await _context.SaveChangesAsync(ct);

        await _identityService.ChangeUserActivationAsync(user.UserId, command.IsActive);
        
        _logger.LogInformation("Employee with Id {userId}, info updated successfully", user.UserId);

        return Result.Updated;
    }
}