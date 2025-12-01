using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.ChangeUserNameAndPassword;

public sealed record class ChangeUserNameAndPasswordCommand(Guid UserId, string Username, string CurrentPassword, string NewPassword) : IRequest<Result<Success>>;