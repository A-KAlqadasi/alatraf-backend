

namespace AlatrafClinic.Application.Features.People.Persons.Services;

public sealed record PersonInput(
    string Fullname,
    DateTime Birthdate,
    string Phone,
    string? NationalNo,
    string Address
);
