using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Identity.Commands.RemovePermissionsFromUser;

public sealed class RemovePermissionsFromUserCommandHandler
    : IRequestHandler<RemovePermissionsFromUserCommand, Result<Success>>
{
    private readonly ILogger<RemovePermissionsFromUserCommandHandler> _logger;
    private readonly IIdentityService _identityService;

    public RemovePermissionsFromUserCommandHandler(
        ILogger<RemovePermissionsFromUserCommandHandler> logger,
        IIdentityService identityService)
    {
        _logger = logger;
        _identityService = identityService;
    }

    public async Task<Result<Success>> Handle(RemovePermissionsFromUserCommand request, CancellationToken ct)
    {
        var result = await _identityService.RemovePermissionsFromUserAsync(
            request.UserId,
            request.PermissionNames,
            ct);

        if (result.IsError)
        {
            _logger.LogError(
                "Error removing permissions '{Permissions}' from user '{UserId}': {ErrorDescription}",
                string.Join(", ", request.PermissionNames),
                request.UserId,
                result.TopError.Description);

            return result.Errors;
        }

        return Result.Success;
    }
}