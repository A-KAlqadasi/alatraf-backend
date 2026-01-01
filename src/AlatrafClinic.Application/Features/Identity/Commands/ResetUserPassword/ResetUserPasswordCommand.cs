using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.ResetUserPassword;

public sealed record ResetUserPasswordCommand(
    string UserId,
    string NewPassword
) : IRequest<Result<Updated>>;
