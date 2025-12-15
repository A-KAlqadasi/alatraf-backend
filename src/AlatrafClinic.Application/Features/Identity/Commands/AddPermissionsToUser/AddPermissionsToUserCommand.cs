using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.AddPermissionsToUser;

public sealed record AddPermissionsToUserCommand(
    string UserId,
    List<string> PermissionNames) : IRequest<Result<Success>>;
