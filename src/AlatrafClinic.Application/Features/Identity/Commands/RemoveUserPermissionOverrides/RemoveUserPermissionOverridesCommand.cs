using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.RemoveUserPermissionOverrides;

public sealed record RemoveUserPermissionOverridesCommand(
    string UserId,
    IReadOnlyCollection<int> PermissionIds
) : IRequest<Result<Deleted>>;
