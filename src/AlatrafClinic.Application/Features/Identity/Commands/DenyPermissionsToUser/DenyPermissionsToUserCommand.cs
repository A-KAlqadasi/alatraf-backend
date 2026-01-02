using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.DenyPermissionsToUser;

public sealed record DenyPermissionsToUserCommand(
    string UserId,
    IReadOnlyCollection<int> PermissionIds
) : IRequest<Result<Updated>>;
