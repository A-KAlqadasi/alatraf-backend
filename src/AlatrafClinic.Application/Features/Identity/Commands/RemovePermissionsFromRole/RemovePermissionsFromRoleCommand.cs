using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.RemovePermissionsFromRole;

public sealed record RemovePermissionsFromRoleCommand(
    string RoleName,
    List<string> PermissionNames) : IRequest<Result<Success>>;