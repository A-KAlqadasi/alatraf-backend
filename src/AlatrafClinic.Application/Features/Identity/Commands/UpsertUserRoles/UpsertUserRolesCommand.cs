using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.UpsertUserRoles;

public sealed record UpsertUserRolesCommand(
    string UserId,
   IReadOnlyCollection<string> RoleIds
) : IRequest<Result<Updated>>;
