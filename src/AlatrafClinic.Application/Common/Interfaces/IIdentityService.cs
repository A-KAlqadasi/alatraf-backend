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
    Task<Result<AppUserDto>> CreateUserAsync(int pesonId, string userName, string password, bool isActive, IList<string> roles, IList<string> permissions);

    Task<Result<bool>> ChangeUserNameAndPasswordAsync(string userId, string newUsername, string newPassword);
    Task<Result<bool>> ChangeUserNameAndPasswordAsync(string userId, string newUsername, string oldPassword, string newPassword);

    public Task<IQueryable<UserDto>> GetUsersAsync();

    public Task<Result<bool>> ChangeUserActivationAsync(string userId, bool isActive);

    // ---------------- USER PERMISSIONS ----------------

    /// <summary>
    /// Adds one or more permissions to a user.
    /// Missing permissions will be created automatically.
    /// Operation is idempotent.
    /// </summary>
    Task<Result<Success>> AddPermissionsToUserAsync(
        string userId,
        IList<string> permissionNames,
        CancellationToken ct = default);

    /// <summary>
    /// Removes one or more permissions from a user.
    /// Operation is idempotent.
    /// </summary>
    Task<Result<Success>> RemovePermissionsFromUserAsync(
        string userId,
        IList<string> permissionNames,
        CancellationToken ct = default);


    // ---------------- ROLE PERMISSIONS ----------------

    /// <summary>
    /// Adds one or more permissions to a role.
    /// Missing permissions will be created automatically.
    /// Operation is idempotent.
    /// </summary>
    Task<Result<Success>> AddPermissionsToRoleAsync(
        string roleName,
        IList<string> permissionNames,
        CancellationToken ct = default);

    /// <summary>
    /// Removes one or more permissions from a role.
    /// Operation is idempotent.
    /// </summary>
    Task<Result<Success>> RemovePermissionsFromRoleAsync(
        string roleName,
        IList<string> permissionNames,
        CancellationToken ct = default);

}