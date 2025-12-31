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

    Task<Result<string>> CreateUserAsync(CreateUserRequest request, CancellationToken ct = default);
    Task<Result<Updated>> ActivateUserAsync(string userId, bool isActive, CancellationToken ct = default);
    Task<Result<Updated>> ResetUserPasswordAsync(ResetPasswordRequest request, CancellationToken ct = default);

    Task<Result<UserDetailsDto>> GetUserByIdAsync(string userId, CancellationToken ct = default);
    Task<Result<IReadOnlyList<UserListItemDto>>> GetUsersAsync(CancellationToken ct = default);

    Task<Result<Updated>> AssignRoleToUserAsync(AssignRoleRequest request, CancellationToken ct = default);
    Task<Result<Deleted>> RemoveRoleFromUserAsync(AssignRoleRequest request, CancellationToken ct = default);
    

    // =========================
    // Role Management
    // =========================
    Task<Result<string>> CreateRoleAsync(CreateRoleRequest request, CancellationToken ct = default);
    Task<Result<Deleted>> DeleteRoleAsync(string roleId, CancellationToken ct = default);

    Task<Result<Updated>> AssignPermissionToRoleAsync(RolePermissionRequest request, CancellationToken ct = default);
    Task<Result<Deleted>> RemovePermissionFromRoleAsync(RolePermissionRequest request, CancellationToken ct = default);

    Task<Result<IReadOnlyList<RoleDetailsDto>>> GetRolesAsync(CancellationToken ct = default);

    // =========================
    // User Permission Overrides
    // =========================
    Task<Result<Updated>> GrantPermissionToUserAsync(UserPermissionOverrideRequest request, CancellationToken ct = default);
    Task<Result<Updated>> DenyPermissionToUserAsync(UserPermissionOverrideRequest request, CancellationToken ct = default);
    Task<Result<Deleted>> RemoveUserPermissionOverrideAsync(UserPermissionOverrideRequest request, CancellationToken ct = default);

    // =========================
    // Permission Queries (VERY IMPORTANT)
    // =========================
    Task<Result<bool>> UserHasPermissionAsync(string userId, string permissionName, CancellationToken ct = default);
    Task<Result<IReadOnlyList<string>>> GetEffectivePermissionsAsync(string userId, CancellationToken ct = default);
}

