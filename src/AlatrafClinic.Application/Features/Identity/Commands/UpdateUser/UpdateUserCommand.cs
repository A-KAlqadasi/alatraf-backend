using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.UpdateUser;

public sealed record class UpdateUserCommand(
    Guid UserId,
    string Fullname,
    DateTime Birthdate,
    string Phone,
    string NationalNo,
    string Address,
    bool Gender,
    bool IsActive
) : IRequest<Result<Updated>>;