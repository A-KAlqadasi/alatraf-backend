using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.UpsertUserPermissions;

public sealed class UpsertUserPermissionsCommandHandler
    : IRequestHandler<UpsertUserPermissionsCommand, Result<Updated>>
{
    private readonly IIdentityService _identityService;

    public UpsertUserPermissionsCommandHandler(IIdentityService identityService)
        => _identityService = identityService;

    public async Task<Result<Updated>> Handle(
        UpsertUserPermissionsCommand request,
        CancellationToken ct)
        => await _identityService.UpsertPermissionsForUserAsync(
            request.UserId,
            request.PermissionIds,
            ct);
}
