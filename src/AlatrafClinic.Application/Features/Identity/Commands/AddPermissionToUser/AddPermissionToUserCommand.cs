using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.AddPermissionToUser;

public sealed record AddPermissionToUserCommand(
    string UserId,
    string PermissionName) : IRequest<Result<bool>>;
