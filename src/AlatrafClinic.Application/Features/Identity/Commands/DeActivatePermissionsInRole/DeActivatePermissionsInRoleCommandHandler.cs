using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.DeActivatePermissionsInRole;

public sealed class DeActivatePermissionsInRoleCommandHandler
    : IRequestHandler<DeActivatePermissionsInRoleCommand, Result<Updated>>
{
    private readonly IIdentityService _identityService;

    public DeActivatePermissionsInRoleCommandHandler(IIdentityService identityService)
        => _identityService = identityService;

    public async Task<Result<Updated>> Handle(
        DeActivatePermissionsInRoleCommand request,
        CancellationToken ct)
        => await _identityService.DeactivateRolePermissionsAsync(
            request.RoleId,
            request.PermissionIds,
            ct);
}
