using AlatrafClinic.Application.Features.People.Dtos;

namespace AlatrafClinic.Application.Features.Identity.Dtos;

public sealed record UserQueryRow(
    string UserId,
    int PersonId,
    PersonDto Person,
    bool IsActive,
    string? UserName
);