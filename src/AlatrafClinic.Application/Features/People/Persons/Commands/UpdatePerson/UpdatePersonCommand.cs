
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Persons.Commands.UpdatePerson;

public sealed record UpdatePersonCommand(
  int PersonId,
  string Fullname,
  DateTime Birthdate,
  string Phone,
  string? NationalNo,
  string Address
) : IRequest<Result<Updated>>;
