using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.RemoveRoleFromUser;

public sealed class RemoveRoleFromUserCommandHandler
    : IRequestHandler<RemoveRoleFromUserCommand, Result<Deleted>>
{
    private readonly IIdentityService _identityService;

    public RemoveRoleFromUserCommandHandler(IIdentityService identityService)
        => _identityService = identityService;

    public Task<Result<Deleted>> Handle(
        RemoveRoleFromUserCommand request,
        CancellationToken ct)
        => _identityService.RemoveRoleFromUserAsync(
            request.UserId,
            request.RoleId,
            ct);
}
