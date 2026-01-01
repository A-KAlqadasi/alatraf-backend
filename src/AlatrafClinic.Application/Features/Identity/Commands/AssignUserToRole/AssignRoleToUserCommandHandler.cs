using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.AssignUserToRole;

public sealed class AssignRoleToUserCommandHandler
    : IRequestHandler<AssignRoleToUserCommand, Result<Updated>>
{
    private readonly IIdentityService _identityService;

    public AssignRoleToUserCommandHandler(IIdentityService identityService)
        => _identityService = identityService;

    public Task<Result<Updated>> Handle(
        AssignRoleToUserCommand request,
        CancellationToken ct)
        => _identityService.AssignRoleToUserAsync(
            request.UserId,
            request.RoleId,
            ct);
}
