using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.ActivatePermissionsInRole;

public sealed record ActivatePermissionsInRoleCommand(
    string RoleId,
    IReadOnlyCollection<int> PermissionIds
) : IRequest<Result<Updated>>;

