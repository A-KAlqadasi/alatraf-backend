using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.AssignRolesToUser;

public sealed class AssignRolesToUserCommandHandler
    : IRequestHandler<AssignRolesToUserCommand, Result<Updated>>
{
    private readonly IIdentityService _identityService;

    public AssignRolesToUserCommandHandler(IIdentityService identityService)
        => _identityService = identityService;

    public Task<Result<Updated>> Handle(
        AssignRolesToUserCommand request,
        CancellationToken ct)
        => _identityService.AssignRolesToUserAsync(
            request.UserId,
            request.RoleIds,
            ct);
}
