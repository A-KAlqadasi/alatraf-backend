using AlatrafClinic.Application.Features.People.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Commands.CreatePerson;

public sealed record CreatePersonCommand(
    string Fullname,
    DateOnly Birthdate,
    string Phone,
    string NationalNo,
    int AddressId, bool Gender) : IRequest<Result<PersonDto>>;
