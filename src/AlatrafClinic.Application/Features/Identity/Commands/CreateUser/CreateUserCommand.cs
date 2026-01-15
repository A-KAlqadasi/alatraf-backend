using AlatrafClinic.Application.Features.Identity.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.CreateUser;

public sealed record CreateUserCommand(
    string Fullname,
    DateOnly Birthdate,
    string Phone,
    string NationalNo,
    int AddressId,
    bool Gender,
    string UserName,
    string Password,
    bool IsActive
) : IRequest<Result<UserCreatedDto>>;
