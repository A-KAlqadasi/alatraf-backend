using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.RemovePermissionsFromUser;

public sealed record RemovePermissionsFromUserCommand(
    string UserId,
    List<string> PermissionNames) : IRequest<Result<Success>>;