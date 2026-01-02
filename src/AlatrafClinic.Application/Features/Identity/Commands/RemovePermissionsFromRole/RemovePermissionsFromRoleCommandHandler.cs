using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.RemovePermissionsFromRole;

public sealed class RemovePermissionsFromRoleCommandHandler
    : IRequestHandler<RemovePermissionsFromRoleCommand, Result<Deleted>>
{
    private readonly IIdentityService _identityService;

    public RemovePermissionsFromRoleCommandHandler(IIdentityService identityService)
        => _identityService = identityService;

    public Task<Result<Deleted>> Handle(
        RemovePermissionsFromRoleCommand request,
        CancellationToken ct)
        => _identityService.RemovePermissionsFromRoleAsync(
            request.RoleId,
            request.PermissionIds,
            ct);
}
