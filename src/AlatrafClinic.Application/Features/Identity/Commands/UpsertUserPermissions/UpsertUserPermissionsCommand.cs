using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.UpsertUserPermissions;

public sealed record UpsertUserPermissionsCommand(
    string UserId,
    IReadOnlyCollection<int> PermissionIds
) : IRequest<Result<Updated>>;
