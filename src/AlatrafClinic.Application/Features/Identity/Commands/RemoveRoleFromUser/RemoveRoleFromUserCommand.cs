using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.RemoveRoleFromUser;

public sealed record RemoveRoleFromUserCommand(
    string UserId,
    string RoleId
) : IRequest<Result<Deleted>>;
