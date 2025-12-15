using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.People.Mappers;
using AlatrafClinic.Application.Features.Identity.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.People;
using MechanicShop.Application.Common.Errors;

using MediatR;

using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Identity.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserDto>>
{
    private readonly ILogger<CreateUserCommandHandler> _logger;
    private readonly IIdentityService _identityService;
    private readonly IAppDbContext _context;

    public CreateUserCommandHandler(ILogger<CreateUserCommandHandler> logger, IIdentityService identityService, IAppDbContext context)
    {
        _logger = logger;
        _identityService = identityService;
        _context = context;
    }
    public async Task<Result<UserDto>> Handle(CreateUserCommand command, CancellationToken ct)
    {
        
        bool isNationalNoExist = await _context.People
            .AnyAsync(p => p.NationalNo == command.NationalNo.Trim(), ct);

        if (isNationalNoExist)
        {
            _logger.LogWarning("National number already exists: {NationalNo}", command.NationalNo);
            return PersonErrors.NationalNoExists;
        }

        bool isPhoneNumberExist = await _context.People
            .AnyAsync(p => p.Phone == command.Phone.Trim(), ct);

        if (isPhoneNumberExist)
        {
            _logger.LogWarning("Phone number already exists: {Phone}", command.Phone);
            return PersonErrors.PhoneExists;
        }
        bool isNameExist = await _context.People
            .AnyAsync(p => p.FullName == command.FullName.Trim(), ct);

        if (isNameExist)
        {
            _logger.LogWarning("Name {fullName} already exist", command.FullName.Trim());
            return PersonErrors.NameIsExist;
        }
        
        
        var createResult = Person.Create(
            command.FullName.Trim(),
            command.Birthdate,
            command.Phone.Trim(),
            command.NationalNo?.Trim(),
            command.Address.Trim(),
            command.Gender);

        if (createResult.IsError)
        {
            return createResult.Errors;
        }

        var person = createResult.Value;

        bool isUserNameExist = await _identityService.IsUserNameExistsAsync(command.UserName.Trim());

        if (isUserNameExist)
        {
            _logger.LogWarning("Username {username} is already exists", command.UserName.Trim());
            return ApplicationErrors.UsernameAlreadyExists;
        }

        await _context.People.AddAsync(person, ct);
        await _context.SaveChangesAsync(ct);

       
        var appUser = await _identityService.CreateUserAsync(person.Id, command.UserName.Trim(), command.Password.Trim(), true, command.Roles, command.Permissions);

        if(appUser.IsError)
        {
            _logger.LogError("Failed to create user for person ID: {personId}. Errors: {errors}", person.Id, appUser.Errors);
            return appUser.Errors;
        }

        var user = appUser.Value;

        _logger.LogInformation("User created successfully with ID: {userId}", user.UserId);

        return new UserDto
        {
            UserId = user.UserId,
            PersonId = person.Id,
            Person = person.ToDto(),
            IsActive = user.IsActive,
            Permissions = user.Permissions.ToList(),
            Roles = user.Roles.ToList()
        };
    }
}