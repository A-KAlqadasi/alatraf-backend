using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Identity.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Identity;
using AlatrafClinic.Infrastructure.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Infrastructure.Identity;

public sealed class IdentityService : IIdentityService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly AlatrafClinicDbContext _db;
    private readonly IUserClaimsPrincipalFactory<AppUser> _userClaimsPrincipalFactory ;
    private readonly IAuthorizationService _authorizationService ;

    public IdentityService(
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager,
        AlatrafClinicDbContext db,
        IUserClaimsPrincipalFactory<AppUser> userClaimsPrincipalFactory,
        IAuthorizationService authorizationService
        )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _db = db;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _authorizationService = authorizationService;
    }

    
    public async Task<Result<UserDetailsDto>> AuthenticateAsync(string userName, string password)
    {
        var user = await _userManager.FindByNameAsync(userName);

        if (user is null)
        {
            return Error.NotFound(
                "User_Not_Found",
                $"User with username '{userName}' not found");
        }

        if (!await _userManager.CheckPasswordAsync(user, password))
        {
            return Error.Conflict(
                "Invalid_Login_Attempt",
                "Username / Password are incorrect");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var permissions = await GetEffectiveUserPermissionsAsync(user.Id, CancellationToken.None);

        var dto = new UserDetailsDto
        {
            UserId = user.Id,
            Username = user.UserName!,
            IsActive = user.IsActive,
            Roles = roles.ToList(),
            Permissions = permissions.Value
        };

        return dto;
    }
    public async Task<bool> AuthorizeAsync(string userId, string? policyName)
    {
        if (string.IsNullOrWhiteSpace(policyName))
        {
            return false;
        }

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return false;
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

        var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        return result.Succeeded;
    }

    public async Task<Result<RefreshToken>> GetRefreshTokenAsync(string refreshToken, string userId)
    {
        var token = await _db.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.UserId == userId);

        if (token is null)
        {
            return Error.NotFound(
                "RefreshToken_Not_Found",
                "Refresh token is invalid.");
        }

        return token;
    }

    // =========================
    // User Management
    // =========================
    public async Task<string?> GetUserNameAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user?.UserName;
    }

    public async Task<Result<string>> CreateUserAsync(int personId, string userName, string password, bool isActive , CancellationToken ct)
    {
        var user = new AppUser
        {
            PersonId = personId,
            IsActive = isActive,
            UserName = userName,
            NormalizedUserName = _userManager.NormalizeName(userName),
            EmailConfirmed = true,
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
            return MyIdentityErrors.FailToCreateUser;

        return user.Id;
    }

    public async Task<Result<Updated>> ActivateUserAsync(string userId, bool isActive, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return MyIdentityErrors.UserNotFound;

        user.IsActive = isActive;
        await _userManager.UpdateAsync(user);

        return Result.Updated;
    }

    public async Task<Result<Updated>> ResetUserPasswordAsync(string userId, string newPassword, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return MyIdentityErrors.UserNotFound;

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

        if (!result.Succeeded)
            return MyIdentityErrors.FailToResetPassword;
        
        return Result.Updated;
    }

    public async Task<Result<Updated>> ChangeUserCredentialsAsync(
        string userId, 
        string oldPassword, 
        string? newPassword = null, 
        string? newUsername = null, 
        CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return MyIdentityErrors.UserNotFound;

        if (!string.IsNullOrWhiteSpace(newPassword))
        {
            var passwordValid = await _userManager.CheckPasswordAsync(user, oldPassword);
            if (!passwordValid)
                return MyIdentityErrors.InvalidCredentials;

            var passwordResult = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);

            if (!passwordResult.Succeeded)
                return MyIdentityErrors.FailToChangePassword;
        }

        if (!string.IsNullOrWhiteSpace(newUsername) && newUsername != user.UserName)
        {
            var existingUser = await _userManager.FindByNameAsync(newUsername);
            if (existingUser != null)
                return MyIdentityErrors.UsernameAlreadyTaken;

            user.UserName = newUsername;
            user.NormalizedUserName = _userManager.NormalizeName(newUsername);

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return MyIdentityErrors.FailToChangeUsername;
        }

        return Result.Updated;
    }

    public async Task<Result<UserDetailsDto>> GetUserByIdAsync(string userId, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return MyIdentityErrors.UserNotFound;

        var roles = await _userManager.GetRolesAsync(user);

        var permissions = await GetEffectiveUserPermissionsAsync(user.Id, ct);

        return new UserDetailsDto
        {
            UserId = user.Id,
            Username = user.UserName!,
            IsActive = user.IsActive,
            Roles = roles.ToList(),
            Permissions = permissions.Value
        };
    }

    public async Task<Result<IReadOnlyList<UserListItemDto>>> GetUsersAsync(CancellationToken ct)
    {
        
        return await _db.Users
            .Select(u => new UserListItemDto
            {
                UserId = u.Id,
                Username = u.UserName!,
                IsActive = u.IsActive,
                PersonName = u.Person.FullName,
                PhoneNumber = u.Person.Phone
            })
            .ToListAsync(ct);
    }

    public async Task<Result<Updated>> AssignRoleToUserAsync(string userId, string roleId, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return MyIdentityErrors.UserNotFound;

        var role = await _roleManager.FindByIdAsync(roleId);

        if (role is null)
            return MyIdentityErrors.RoleNotFound;
        
        if(await _userManager.IsInRoleAsync(user, role.Name!))
            return Result.Updated;

        var result = await _userManager.AddToRoleAsync(user, role.Name!);
        
        if (!result.Succeeded)
            return MyIdentityErrors.FaliedToAssignRoleToUser;


        return Result.Updated;
    }

    public async Task<Result<Deleted>> RemoveRoleFromUserAsync(string userId, string roleId, CancellationToken ct)
    {
       var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return MyIdentityErrors.UserNotFound;

        var role = await _roleManager.FindByIdAsync(roleId);

        if (role is null)
            return MyIdentityErrors.RoleNotFound;

        await _userManager.RemoveFromRoleAsync(user, role.Name!);

        return Result.Deleted;
    }

    // =========================
    // Role Management
    // =========================
    public async Task<Result<string>> CreateRoleAsync(string name, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(name))
            return MyIdentityErrors.RoleNameIsRequired;

        var normalizedName = _roleManager.NormalizeKey(name);

        var existingRole = await _roleManager.Roles
            .FirstOrDefaultAsync(r => r.NormalizedName == normalizedName, ct);

        if (existingRole is not null)
            return existingRole.Id;

        var role = new IdentityRole(name);

        var result = await _roleManager.CreateAsync(role);
        if (!result.Succeeded)
            return MyIdentityErrors.FailToCreateRole;

        return role.Id;
    }

    public async Task<Result<Deleted>> DeleteRoleAsync(string roleId, CancellationToken ct)
    {
        var role = await _roleManager.FindByIdAsync(roleId);

        if (role is null)
            return MyIdentityErrors.RoleNotFound;
        
        var users = await _userManager.GetUsersInRoleAsync(role.Name!);
        if (users.Any())
            return MyIdentityErrors.RoleAssignedToUsers;

        await _roleManager.DeleteAsync(role);
        return Result.Deleted;
    }

    public async Task<Result<Updated>> AssignPermissionsToRoleAsync(
    string roleId,
    IReadOnlyCollection<int> permissionIds,
    CancellationToken ct = default)
    {
        if (permissionIds is null || permissionIds.Count == 0)
            return Result.Updated;

        var role = await _roleManager.FindByIdAsync(roleId);
        if (role is null)
            return MyIdentityErrors.RoleNotFound;

        var existingPermissions = await _db.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .Select(rp => rp.PermissionId)
            .ToListAsync(ct);

        var newPermissions = permissionIds
            .Except(existingPermissions)
            .Distinct()
            .Select(pid => new RolePermission
            {
                RoleId = roleId,
                PermissionId = pid
            })
            .ToList();

        if (newPermissions.Count == 0)
            return Result.Updated;

        await _db.RolePermissions.AddRangeAsync(newPermissions, ct);
        await _db.SaveChangesAsync(ct);

        return Result.Updated;
    }


   public async Task<Result<Deleted>> RemovePermissionsFromRoleAsync(
    string roleId,
    IReadOnlyCollection<int> permissionIds,
    CancellationToken ct = default)
    {
        if (permissionIds is null || permissionIds.Count == 0)
            return Result.Deleted;

        var role = await _roleManager.FindByIdAsync(roleId);
        if (role is null)
            return MyIdentityErrors.RoleNotFound;

        var rolePermissions = await _db.RolePermissions
            .Where(rp => rp.RoleId == roleId && permissionIds.Contains(rp.PermissionId))
            .ToListAsync(ct);

        if (rolePermissions.Count == 0)
            return Result.Deleted;

        _db.RolePermissions.RemoveRange(rolePermissions);
        await _db.SaveChangesAsync(ct);

        return Result.Deleted;
    }


    public async Task<Result<IReadOnlyList<RoleDetailsDto>>> GetRolesAsync(CancellationToken ct)
    {
        return await _roleManager.Roles
            .Select(r => new RoleDetailsDto
            {
                RoleId = r.Id,
                Name = r.Name!,
                Permissions = _db.Set<RolePermission>()
                    .Where(rp => rp.RoleId == r.Id)
                    .Select(rp => rp.Permission.Name)
                    .ToList()
            })
            .ToListAsync(ct);
    }

    // =========================
    // User Permission Overrides
    // =========================
    
    public Task<Result<Updated>> GrantPermissionsToUserAsync(
    string userId,
    IReadOnlyCollection<int> permissionIds,
    CancellationToken ct)
    => SetUserPermissionOverrides(userId, permissionIds, Effect.Grant, ct);

    public Task<Result<Updated>> DenyPermissionsToUserAsync(
    string userId,
    IReadOnlyCollection<int> permissionIds,
    CancellationToken ct)
    => SetUserPermissionOverrides(userId, permissionIds, Effect.Deny, ct);


    public async Task<Result<Deleted>> RemoveUserPermissionOverridesAsync(
    string userId,
    IReadOnlyCollection<int> permissionIds,
    CancellationToken ct)
    {
        if (permissionIds is null || permissionIds.Count == 0)
            return Result.Deleted;

        var overrides = await _db.Set<UserPermissionOverride>()
            .Where(x =>
                x.UserId == userId &&
                permissionIds.Contains(x.PermissionId))
            .ToListAsync(ct);

        if (overrides.Count == 0)
            return Result.Deleted; // idempotent

        _db.RemoveRange(overrides);
        await _db.SaveChangesAsync(ct);

        return Result.Deleted;
    }

    private async Task<Result<Updated>> SetUserPermissionOverrides(
    string userId,
    IReadOnlyCollection<int> permissionIds,
    Effect effect,
    CancellationToken ct)
    {
        if (permissionIds is null || permissionIds.Count == 0)
            return Result.Updated;

        var existingOverrides = await _db.Set<UserPermissionOverride>()
            .Where(x =>
                x.UserId == userId &&
                permissionIds.Contains(x.PermissionId))
            .ToListAsync(ct);

        var existingMap = existingOverrides
            .ToDictionary(x => x.PermissionId);

        foreach (var permissionId in permissionIds.Distinct())
        {
            if (existingMap.TryGetValue(permissionId, out var existing))
            {
                if (existing.Effect != effect)
                    existing.Effect = effect;
            }
            else
            {
                _db.Add(new UserPermissionOverride
                {
                    UserId = userId,
                    PermissionId = permissionId,
                    Effect = effect
                });
            }
        }

        await _db.SaveChangesAsync(ct);
        return Result.Updated;
    }


    // =========================
    // Permission Queries
    // =========================
    public async Task<Result<bool>> UserHasPermissionAsync(
        string userId,
        string permissionName,
        CancellationToken ct)
    {
        var permission = await _db.Set<ApplicationPermission>()
            .SingleAsync(p => p.Name == permissionName.ToLower(), ct);

        var deny = await _db.Set<UserPermissionOverride>()
            .AnyAsync(x =>
                x.UserId == userId &&
                x.PermissionId == permission.Id &&
                x.Effect == Effect.Deny, ct);

        if (deny) return false;

        var grant = await _db.Set<UserPermissionOverride>()
            .AnyAsync(x =>
                x.UserId == userId &&
                x.PermissionId == permission.Id &&
                x.Effect == Effect.Grant, ct);

        if (grant) return true;

        return await _db.Set<RolePermission>()
            .AnyAsync(rp =>
                rp.PermissionId == permission.Id &&
                _db.UserRoles.Any(ur =>
                    ur.UserId == userId &&
                    ur.RoleId == rp.RoleId), ct);
    }

    public async Task<Result<IReadOnlyList<string>>> GetEffectiveUserPermissionsAsync(
        string userId,
        CancellationToken ct)
    {
        var rolePerms =
            from ur in _db.UserRoles
            join rp in _db.Set<RolePermission>() on ur.RoleId equals rp.RoleId
            join p in _db.Set<ApplicationPermission>() on rp.PermissionId equals p.Id
            where ur.UserId == userId
            select p.Name;

        var grants =
            from up in _db.Set<UserPermissionOverride>()
            join p in _db.Set<ApplicationPermission>() on up.PermissionId equals p.Id
            where up.UserId == userId && up.Effect == Effect.Grant
            select p.Name;

        var denies =
            from up in _db.Set<UserPermissionOverride>()
            join p in _db.Set<ApplicationPermission>() on up.PermissionId equals p.Id
            where up.UserId == userId && up.Effect == Effect.Deny
            select p.Name;

        return await rolePerms
            .Union(grants)
            .Except(denies)
            .Distinct()
            .ToListAsync(ct);
    }
    
    public async Task<Result<IReadOnlyList<PermissionDto>>> GetAllPermissionsAsync(CancellationToken ct)
    {
        return await _db.Set<ApplicationPermission>()
            .Select(p => new PermissionDto
            {
                PermissionId = p.Id,
                Name = p.Name,
                Description = p.Description
            })
            .ToListAsync(ct);
    }
}