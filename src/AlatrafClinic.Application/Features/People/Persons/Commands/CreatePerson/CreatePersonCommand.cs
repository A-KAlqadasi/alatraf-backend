

using AlatrafClinic.Application.Features.People.Persons.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Persons.Commands.CreatePerson;

public sealed record CreatePersonCommand(
    string Fullname,
    DateTime? Birthdate,
    string? Phone,
    string? NationalNo,
    string? Address) : IRequest<Result<PersonDto>>;
