using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.DenyPermissionsToUser;

public sealed class DenyPermissionsToUserCommandHandler
    : IRequestHandler<DenyPermissionsToUserCommand, Result<Updated>>
{
    private readonly IIdentityService _identityService;

    public DenyPermissionsToUserCommandHandler(IIdentityService identityService)
        => _identityService = identityService;

    public Task<Result<Updated>> Handle(
        DenyPermissionsToUserCommand request,
        CancellationToken ct)
        => _identityService.DenyPermissionsToUserAsync(
            request.UserId,
            request.PermissionIds,
            ct);
}
