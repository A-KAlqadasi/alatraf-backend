using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.RemovePermissionFromRole;

public sealed record RemovePermissionFromRoleCommand(
    string RoleName,
    string PermissionName) : IRequest<Result<bool>>;
