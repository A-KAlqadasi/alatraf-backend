using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.GrantPermissionsToUser;

public sealed class GrantPermissionsToUserCommandHandler
    : IRequestHandler<GrantPermissionsToUserCommand, Result<Updated>>
{
    private readonly IIdentityService _identityService;

    public GrantPermissionsToUserCommandHandler(IIdentityService identityService)
        => _identityService = identityService;

    public Task<Result<Updated>> Handle(
        GrantPermissionsToUserCommand request,
        CancellationToken ct)
        => _identityService.GrantPermissionsToUserAsync(
            request.UserId,
            request.PermissionIds,
            ct);
}
