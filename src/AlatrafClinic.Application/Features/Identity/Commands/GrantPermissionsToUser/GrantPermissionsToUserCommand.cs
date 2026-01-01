using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.GrantPermissionsToUser;

public sealed record GrantPermissionsToUserCommand(
    string UserId,
    IReadOnlyCollection<int> PermissionIds
) : IRequest<Result<Updated>>;
