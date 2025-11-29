using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.RemovePermissionFromUser;

public sealed record RemovePermissionFromUserCommand(
    string UserId,
    string PermissionName) : IRequest<Result<bool>>;
