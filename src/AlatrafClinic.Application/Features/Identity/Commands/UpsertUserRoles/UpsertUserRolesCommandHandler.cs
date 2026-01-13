using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.UpsertUserRoles;

public sealed class UpsertUserRolesCommandHandler
    : IRequestHandler<UpsertUserRolesCommand, Result<Updated>>
{
    private readonly IIdentityService _identityService;

    public UpsertUserRolesCommandHandler(IIdentityService identityService)
        => _identityService = identityService;

    public Task<Result<Updated>> Handle(
        UpsertUserRolesCommand request,
        CancellationToken ct)
        => _identityService.UpsertUserRolesAsync(
            request.UserId,
            request.RoleIds,
            ct);
}