using AlatrafClinic.Application.Features.Identity.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Identity;

namespace AlatrafClinic.Application.Common.Interfaces;

public interface IIdentityService
{
    // =========================
    // Authentication
    // =========================
    Task<bool> AuthorizeAsync(string userId, string policyName);
    Task<Result<UserDetailsDto>> AuthenticateAsync(string userName, string password);
    Task<Result<RefreshToken>> GetRefreshTokenAsync(string refreshToken, string userId);

    Task<string?> GetUserNameAsync(string userId);

    Task<Result<string>> CreateUserAsync(int personId, string userName, string password, bool isActive , CancellationToken ct);
    Task<Result<Updated>> ActivateUserAsync(string userId, bool isActive, CancellationToken ct = default);
    Task<Result<Updated>> ResetUserPasswordAsync(string userId, string newPassword, CancellationToken ct = default);
    Task<Result<Updated>> ChangeUserCredentialsAsync(
        string userId, 
        string oldPassword, 
        string? newPassword = null, 
        string? newUsername = null, 
        CancellationToken ct = default);

    Task<Result<UserDetailsDto>> GetUserByIdAsync(string userId, CancellationToken ct = default);
    Task<Result<IReadOnlyList<UserListItemDto>>> GetUsersAsync(string? searchBy, bool? isActive, CancellationToken ct = default);
    Task<Result<Updated>> UpsertUserRolesAsync(
    string userId,
    IReadOnlyCollection<string> roleIds,
    CancellationToken ct);
    

    // =========================
    // Role Management
    // =========================

    Task<Result<IReadOnlyList<RoleDetailsDto>>> GetRolesAsync(CancellationToken ct = default);

    // =========================
    // Permission Queries (VERY IMPORTANT)
    // =========================
    Task<Result<bool>> UserHasPermissionAsync(string userId, string permissionName, CancellationToken ct = default);
    Task<Result<IReadOnlyList<string>>> GetEffectiveUserPermissionsAsync(string userId, CancellationToken ct = default);
    Task<Result<IReadOnlyList<PermissionDto>>> GetAllPermissionsAsync(string? search, CancellationToken ct = default);
    
    Task<Result<Updated>> ActivateRolePermissionsAsync(
        string roleId,
        IReadOnlyCollection<int> permissionIds,
        CancellationToken ct = default);
    
    Task<Result<Updated>> DeactivateRolePermissionsAsync(
        string roleId,
        IReadOnlyCollection<int> permissionIds,
        CancellationToken ct = default);
    Task<bool> IsUserNameExistsAsync(string userName, CancellationToken ct = default);
    
    Task<Result<Updated>> UpsertPermissionsForUserAsync(
    string userId,
    IReadOnlyCollection<int> permissionIds,
    CancellationToken ct);
    
}