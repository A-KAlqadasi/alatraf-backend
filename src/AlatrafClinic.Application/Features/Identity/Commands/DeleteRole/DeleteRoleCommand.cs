using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.DeleteRole;

public sealed record DeleteRoleCommand(
    string RoleId
) : IRequest<Result<Deleted>>;
