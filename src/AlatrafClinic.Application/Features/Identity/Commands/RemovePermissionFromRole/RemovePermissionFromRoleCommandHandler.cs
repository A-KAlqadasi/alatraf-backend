using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Identity.Commands.RemovePermissionFromRole;

public sealed class RemovePermissionFromRoleCommandHandler
    : IRequestHandler<RemovePermissionFromRoleCommand, Result<bool>>
{
    private readonly ILogger<RemovePermissionFromRoleCommandHandler> _logger;
    private readonly IIdentityService _identityService;

    public RemovePermissionFromRoleCommandHandler(
        ILogger<RemovePermissionFromRoleCommandHandler> logger,
        IIdentityService identityService)
    {
        _logger = logger;
        _identityService = identityService;
    }

    public async Task<Result<bool>> Handle(RemovePermissionFromRoleCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.RemovePermissionFromRoleAsync(
            request.RoleName,
            request.PermissionName,
            cancellationToken);

        if (result.IsError)
        {
            _logger.LogError(
                "Error removing permission '{Permission}' from role '{Role}': {ErrorDescription}",
                request.PermissionName,
                request.RoleName,
                result.TopError.Description);

            return result.Errors;
        }

        return true;
    }
}