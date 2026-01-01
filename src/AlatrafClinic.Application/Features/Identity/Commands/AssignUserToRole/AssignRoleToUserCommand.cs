using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.AssignUserToRole;

public sealed record AssignRoleToUserCommand(
    string UserId,
    string RoleId
) : IRequest<Result<Updated>>;
