using AlatrafClinic.Application.Features.Identity.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.CreateUser;

public sealed record class CreateUserCommand(
    string FullName,
    DateOnly Birthdate,
    string Phone,
    string NationalNo,
    string Address,
    bool Gender,
    string UserName,
    string Password,
    List<string> Permissions,
    List<string> Roles 
    ) : IRequest<Result<UserDto>>;