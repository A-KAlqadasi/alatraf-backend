using AlatrafClinic.Application.Features.People.Persons.Dtos;

namespace AlatrafClinic.Application.Features.Identity.Dtos;

public sealed class UserDto
{
    public string UserId { get; set; } = string.Empty;
    public int PersonId { get; set; }
    public PersonDto? Person { get; set; }
    public bool IsActive { get; set; }
    public string? UserName {get; set; }
    public List<string>? Permissions {get; set;}
    public List<string>? Roles { get; set; }
}