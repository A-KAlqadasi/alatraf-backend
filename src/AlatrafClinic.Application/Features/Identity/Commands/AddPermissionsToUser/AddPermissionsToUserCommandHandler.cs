using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Identity.Commands.AddPermissionsToUser;

public sealed class AddPermissionsToUserCommandHandler
    : IRequestHandler<AddPermissionsToUserCommand, Result<Success>>
{
    private readonly ILogger<AddPermissionsToUserCommandHandler> _logger;
    private readonly IIdentityService _identityService;

    public AddPermissionsToUserCommandHandler(
        ILogger<AddPermissionsToUserCommandHandler> logger,
        IIdentityService identityService)
    {
        _logger = logger;
        _identityService = identityService;
    }

    public async Task<Result<Success>> Handle(AddPermissionsToUserCommand request, CancellationToken ct)
    {

        var result = await _identityService.AddPermissionsToUserAsync(
            request.UserId,
            request.PermissionNames,
            ct);

        if (result.IsError)
        {
            _logger.LogError(
                "Error adding permissions '{Permissions}' to user '{UserId}': {ErrorDescription}",
                string.Join(", ", request.PermissionNames),
                request.UserId,
                result.TopError.Description);

            return result.Errors;
        }

        return Result.Success;
    }
}