
namespace AlatrafClinic.Application.Features.Identity.Dtos;

public sealed record AppUserDto(string UserId, string UserName, IList<string> Roles, IList<string> Permissions);