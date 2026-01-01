using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.AssignPermissionsToRole;

public sealed record AssignPermissionsToRoleCommand(
    string RoleId,
    IReadOnlyCollection<int> PermissionIds
) : IRequest<Result<Updated>>;
