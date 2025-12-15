using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Identity.Commands.AddPermissionsToRole;

public sealed class AddPermissionsToRoleCommandHandler
    : IRequestHandler<AddPermissionsToRoleCommand, Result<Success>>
{
    private readonly ILogger<AddPermissionsToRoleCommandHandler> _logger;
    private readonly IIdentityService _identityService;

    public AddPermissionsToRoleCommandHandler(
        ILogger<AddPermissionsToRoleCommandHandler> logger,
        IIdentityService identityService)
    {
        _logger = logger;
        _identityService = identityService;
    }

    public async Task<Result<Success>> Handle(AddPermissionsToRoleCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.AddPermissionsToRoleAsync(
            request.RoleName,
            request.PermissionNames,
            cancellationToken);

        if (result.IsError)
        {
            _logger.LogError(
                "Error adding permissions '{Permissions}' to role '{Role}': {ErrorDescription}",
                string.Join(", ", request.PermissionNames),
                request.RoleName,
                result.TopError.Description);

            return result.Errors;
        }

        return Result.Success;
    }
}