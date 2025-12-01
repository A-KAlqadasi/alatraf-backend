using Microsoft.Extensions.Logging;

using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using MechanicShop.Application.Common.Errors;

using MediatR;
using AlatrafClinic.Application.Common.Interfaces;

namespace AlatrafClinic.Application.Features.Identity.Commands.ChangeUserNameAndPassword;

public class ChangeUserNameAndPasswordCommandHandler : IRequestHandler<ChangeUserNameAndPasswordCommand, Result<Success>>
{
    private readonly ILogger<ChangeUserNameAndPasswordCommandHandler> _logger;
    private readonly IIdentityService _identityService;

    public ChangeUserNameAndPasswordCommandHandler(ILogger<ChangeUserNameAndPasswordCommandHandler> logger, IIdentityService identityService)
    {
        _logger = logger;
        _identityService = identityService;
    }

    public async Task<Result<Success>> Handle(ChangeUserNameAndPasswordCommand command, CancellationToken cancellationToken)
    {
        var authResult = await _identityService.AuthenticateAsync(command.Username, command.CurrentPassword);

        if (!authResult.IsSuccess)
        {
            _logger.LogError("Username or password is incorrect");
            return ApplicationErrors.AuthenticationFailed;
        }

        var appUser = await _identityService.GetUserByIdAsync(command.UserId.ToString());

        if (appUser.IsError)
        {
            return appUser.Errors;
        }
        var user = appUser.Value;
        
        var isUserExists = await _identityService.IsUserNameExistsAsync(command.Username);
        
        if(isUserExists && user.UserName!.Trim() != command.Username.Trim())
        {
            _logger.LogWarning("Username {username} is already exists", command.Username);
            return ApplicationErrors.UsernameAlreadyExists;
        }

        var changeResult = await _identityService.ChangeUserNameAndPasswordAsync(command.UserId.ToString(), command.Username, command.NewPassword);

        if (!changeResult.IsSuccess)
        {
            _logger.LogError("Failed to change username/password");
            return changeResult.Errors;
        }

        _logger.LogInformation("Username and password changed successfully for userId {userId}", command.UserId);
        
        return Result.Success;
    }
}