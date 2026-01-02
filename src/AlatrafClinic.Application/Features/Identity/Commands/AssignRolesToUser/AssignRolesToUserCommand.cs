using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.AssignRolesToUser;

public sealed record AssignRolesToUserCommand(
    string UserId,
   IReadOnlyCollection<string> RoleIds
) : IRequest<Result<Updated>>;
