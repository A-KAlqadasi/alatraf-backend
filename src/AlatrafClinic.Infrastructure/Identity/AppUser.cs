using AlatrafClinic.Domain.People;

using Microsoft.AspNetCore.Identity;

namespace AlatrafClinic.Infrastructure.Identity;

public class AppUser : IdentityUser
{
    public int PersonId { get; set; }
    public Person Person { get; set; } = default!;
    public bool IsActive { get; set; }
}