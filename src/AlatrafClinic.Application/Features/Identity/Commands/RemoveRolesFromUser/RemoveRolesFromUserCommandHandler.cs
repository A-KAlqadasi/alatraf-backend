using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.RemoveRolesFromUser;

public sealed class RemoveRolesFromUserCommandHandler
    : IRequestHandler<RemoveRolesFromUserCommand, Result<Deleted>>
{
    private readonly IIdentityService _identityService;

    public RemoveRolesFromUserCommandHandler(IIdentityService identityService)
        => _identityService = identityService;

    public Task<Result<Deleted>> Handle(
        RemoveRolesFromUserCommand request,
        CancellationToken ct)
        => _identityService.RemoveRolesFromUserAsync(
            request.UserId,
            request.RoleIds,
            ct);
}
