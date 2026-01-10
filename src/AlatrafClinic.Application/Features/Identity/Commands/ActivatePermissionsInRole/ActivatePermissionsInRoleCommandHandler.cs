using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.ActivatePermissionsInRole;

public sealed class ActivatePermissionsInRoleCommandHandler
    : IRequestHandler<ActivatePermissionsInRoleCommand, Result<Updated>>
{
    private readonly IIdentityService _identityService;

    public ActivatePermissionsInRoleCommandHandler(IIdentityService identityService)
        => _identityService = identityService;

    public async Task<Result<Updated>> Handle(
        ActivatePermissionsInRoleCommand request,
        CancellationToken ct)
        => await _identityService.ActivateRolePermissionsAsync(
            request.RoleId,
            request.PermissionIds,
            ct);
}
