using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.AssignPermissionsToRole;

public sealed class AssignPermissionsToRoleCommandHandler
    : IRequestHandler<AssignPermissionsToRoleCommand, Result<Updated>>
{
    private readonly IIdentityService _identityService;

    public AssignPermissionsToRoleCommandHandler(IIdentityService identityService)
        => _identityService = identityService;

    public Task<Result<Updated>> Handle(
        AssignPermissionsToRoleCommand request,
        CancellationToken ct)
        => _identityService.AssignPermissionsToRoleAsync(
            request.RoleId,
            request.PermissionIds,
            ct);
}
