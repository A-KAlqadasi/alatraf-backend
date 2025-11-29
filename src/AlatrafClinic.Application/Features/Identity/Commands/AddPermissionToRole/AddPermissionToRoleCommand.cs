using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.AddPermissionToRole;

public sealed record AddPermissionToRoleCommand(
    string RoleName,
    string PermissionName) : IRequest<Result<bool>>;
