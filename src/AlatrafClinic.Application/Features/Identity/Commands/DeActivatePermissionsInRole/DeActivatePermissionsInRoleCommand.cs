using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.DeActivatePermissionsInRole;

public sealed record DeActivatePermissionsInRoleCommand(
    string RoleId,
    IReadOnlyCollection<int> PermissionIds
) : IRequest<Result<Updated>>;

