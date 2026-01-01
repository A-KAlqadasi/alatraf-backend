using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.RemoveUserPermissionOverrides;

public sealed class RemoveUserPermissionOverridesCommandHandler
    : IRequestHandler<RemoveUserPermissionOverridesCommand, Result<Deleted>>
{
    private readonly IIdentityService _identityService;

    public RemoveUserPermissionOverridesCommandHandler(IIdentityService identityService)
        => _identityService = identityService;

    public Task<Result<Deleted>> Handle(
        RemoveUserPermissionOverridesCommand request,
        CancellationToken ct)
        => _identityService.RemoveUserPermissionOverridesAsync(
            request.UserId,
            request.PermissionIds,
            ct);
}
