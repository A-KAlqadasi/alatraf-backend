using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.RemovePermissionsFromRole;

public sealed record RemovePermissionsFromRoleCommand(
    string RoleId,
    IReadOnlyCollection<int> PermissionIds
) : IRequest<Result<Deleted>>;

