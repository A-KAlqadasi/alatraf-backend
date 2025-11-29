using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Identity.Commands.AddPermissionToRole;

public sealed class AddPermissionToRoleCommandHandler
    : IRequestHandler<AddPermissionToRoleCommand, Result<bool>>
{
    private readonly ILogger<AddPermissionToRoleCommandHandler> _logger;
    private readonly IIdentityService _identityService;

    public AddPermissionToRoleCommandHandler(
        ILogger<AddPermissionToRoleCommandHandler> logger,
        IIdentityService identityService)
    {
        _logger = logger;
        _identityService = identityService;
    }

    public async Task<Result<bool>> Handle(AddPermissionToRoleCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.AddPermissionToRoleAsync(
            request.RoleName,
            request.PermissionName,
            cancellationToken);

        if (result.IsError)
        {
            _logger.LogError(
                "Error adding permission '{Permission}' to role '{Role}': {ErrorDescription}",
                request.PermissionName,
                request.RoleName,
                result.TopError.Description);

            return result.Errors;
        }

        return true;
    }
}