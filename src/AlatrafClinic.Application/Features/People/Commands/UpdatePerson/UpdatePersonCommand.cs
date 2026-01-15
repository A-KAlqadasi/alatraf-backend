using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Commands.UpdatePerson;

public sealed record UpdatePersonCommand(
  int PersonId,
  string Fullname,
  DateOnly Birthdate,
  string Phone,
  string? NationalNo,
  int AddressId,
  bool Gender
) : IRequest<Result<Updated>>;
