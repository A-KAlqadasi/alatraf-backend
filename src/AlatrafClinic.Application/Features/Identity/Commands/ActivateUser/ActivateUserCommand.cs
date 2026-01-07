using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.ActivateUser;

public sealed record ActivateUserCommand(
    string UserId,
    bool IsActive
) : IRequest<Result<Updated>>;
