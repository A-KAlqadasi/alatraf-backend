using AlatrafClinic.Application.Features.Identity.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Identity;

namespace AlatrafClinic.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<bool> IsInRoleAsync(string userId, string role);

    Task<bool> AuthorizeAsync(string userId, string? policyName);

    Task<Result<AppUserDto>> AuthenticateAsync(string userName, string password);

    public Task<Result<UserDto>> GetUserByIdAsync(string userId);

    Task<string?> GetUserNameAsync(string userId);
    Task<bool> IsUserNameExistsAsync(string userName);
    Task<Result<RefreshToken>> GetRefreshTokenAsync(string refreshToken, string userId);

    Task<Result<bool>> AddPermissionToRoleAsync(string roleName, string permissionName, CancellationToken ct = default);
    Task<Result<bool>> RemovePermissionFromRoleAsync(string roleName, string permissionName, CancellationToken ct = default);

    Task<Result<bool>> AddPermissionToUserAsync(string userId, string permissionName, CancellationToken ct = default);
    Task<Result<bool>> RemovePermissionFromUserAsync(string userId, string permissionName, CancellationToken ct = default);

    Task<Result<AppUserDto>> CreateUserAsync(int pesonId, string userName, string password, bool isActive, IList<string> roles, IList<string> permissions);

    Task<Result<bool>> ChangeUserNameAndPasswordAsync(string userId, string newUsername, string newPassword);
    Task<Result<bool>> ChangeUserNameAndPasswordAsync(string userId, string newUsername, string oldPassword, string newPassword);

    public Task<IQueryable<UserDto>> GetUsersAsync();

    public Task<Result<bool>> ChangeUserActivationAsync(string userId, bool isActive);

}