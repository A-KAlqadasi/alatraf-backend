using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.RemoveRolesFromUser;

public sealed record RemoveRolesFromUserCommand(
    string UserId,
    IReadOnlyCollection<string> RoleIds
) : IRequest<Result<Deleted>>;
