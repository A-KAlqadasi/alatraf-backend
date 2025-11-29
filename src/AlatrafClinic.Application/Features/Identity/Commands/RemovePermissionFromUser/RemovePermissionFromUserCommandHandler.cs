using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Identity.Commands.RemovePermissionFromUser;

public sealed class RemovePermissionFromUserCommandHandler
    : IRequestHandler<RemovePermissionFromUserCommand, Result<bool>>
{
    private readonly ILogger<RemovePermissionFromUserCommandHandler> _logger;
    private readonly IIdentityService _identityService;

    public RemovePermissionFromUserCommandHandler(
        ILogger<RemovePermissionFromUserCommandHandler> logger,
        IIdentityService identityService)
    {
        _logger = logger;
        _identityService = identityService;
    }

    public async Task<Result<bool>> Handle(RemovePermissionFromUserCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.RemovePermissionFromUserAsync(
            request.UserId,
            request.PermissionName,
            cancellationToken);

        if (result.IsError)
        {
            _logger.LogError(
                "Error removing permission '{Permission}' from user '{UserId}': {ErrorDescription}",
                request.PermissionName,
                request.UserId,
                result.TopError.Description);

            return result.Errors;
        }

        return true;
    }
}