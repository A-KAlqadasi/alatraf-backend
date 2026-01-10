using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Identity.Dtos;
using AlatrafClinic.Application.Features.People.Services.CreatePerson;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Identity.Commands.CreateUser;

public sealed class CreateUserCommandHandler
    : IRequestHandler<CreateUserCommand, Result<UserCreatedDto>>
{
    private readonly IAppDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IPersonCreateService _personCreate;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public CreateUserCommandHandler(IAppDbContext context, IIdentityService identityService, IPersonCreateService personCreate, ILogger<CreateUserCommandHandler> logger)
    {
        _context = context;
        _identityService = identityService;
        _personCreate = personCreate;
        _logger = logger;
    }

    public async Task<Result<UserCreatedDto>> Handle(
        CreateUserCommand command,
        CancellationToken ct)
    {
        var isUserNameExists = await _identityService.IsUserNameExistsAsync(command.UserName, ct);
        if (isUserNameExists)
        {
            return Error.Conflict(
                nameof(command.UserName),
                "Username already exists.");
        }

        var personResult = await _personCreate.CreateAsync(
            command.Fullname,
            command.Birthdate,
            command.Phone,
            command.NationalNo,
            command.Address,
            command.Gender,
            ct);

        if (personResult.IsError)
        {
            _logger.LogError("Failed to create person: {Errors}", personResult.Errors);
            return personResult.Errors;
        }

        await _context.People.AddAsync(personResult.Value, ct);
        await _context.SaveChangesAsync(ct);
        
        var result = await _identityService.CreateUserAsync(
            personResult.Value.Id,
            command.UserName,
            command.Password,
            command.IsActive,
            ct);

        return new UserCreatedDto
        {
            UserId = result.Value
        };
    }
}
