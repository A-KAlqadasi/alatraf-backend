using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.AddPermissionsToRole;

public sealed record AddPermissionsToRoleCommand(
    string RoleName,
    IList<string> PermissionNames) : IRequest<Result<Success>>;
