using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.ChangeUserCredentials;

public sealed record ChangeUserCredentialsCommand(
    string UserId,
    string OldPassword,
    string? NewPassword,
    string? NewUsername
) : IRequest<Result<Updated>>;
