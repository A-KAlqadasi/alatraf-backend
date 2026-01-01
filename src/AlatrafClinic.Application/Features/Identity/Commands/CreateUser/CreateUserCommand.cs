using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.CreateUser;

public sealed record CreateUserCommand(
    int PersonId,
    string UserName,
    string Password,
    bool IsActive
) : IRequest<Result<string>>;
