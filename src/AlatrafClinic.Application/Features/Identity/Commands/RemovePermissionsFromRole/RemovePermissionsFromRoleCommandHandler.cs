using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Identity.Commands.RemovePermissionsFromRole;

public sealed class RemovePermissionsFromRoleCommandHandler
    : IRequestHandler<RemovePermissionsFromRoleCommand, Result<Success>>
{
    private readonly ILogger<RemovePermissionsFromRoleCommandHandler> _logger;
    private readonly IIdentityService _identityService;

    public RemovePermissionsFromRoleCommandHandler(
        ILogger<RemovePermissionsFromRoleCommandHandler> logger,
        IIdentityService identityService)
    {
        _logger = logger;
        _identityService = identityService;
    }

    public async Task<Result<Success>> Handle(RemovePermissionsFromRoleCommand request, CancellationToken ct)
    {
        var result = await _identityService.RemovePermissionsFromRoleAsync(
            request.RoleName,
            request.PermissionNames,
            ct);

        if (result.IsError)
        {
            _logger.LogError(
                "Error removing permissions '{Permissions}' from role '{Role}': {ErrorDescription}",
                string.Join(", ", request.PermissionNames),
                request.RoleName,
                result.TopError.Description);

            return result.Errors;
        }

        return Result.Success;
    }
}