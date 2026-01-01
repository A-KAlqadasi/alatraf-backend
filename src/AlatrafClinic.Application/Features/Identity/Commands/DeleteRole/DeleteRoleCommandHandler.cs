using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.DeleteRole;

public sealed class DeleteRoleCommandHandler
    : IRequestHandler<DeleteRoleCommand, Result<Deleted>>
{
    private readonly IIdentityService _identityService;

    public DeleteRoleCommandHandler(IIdentityService identityService)
        => _identityService = identityService;

    public Task<Result<Deleted>> Handle(
        DeleteRoleCommand request,
        CancellationToken ct)
        => _identityService.DeleteRoleAsync(request.RoleId, ct);
}
