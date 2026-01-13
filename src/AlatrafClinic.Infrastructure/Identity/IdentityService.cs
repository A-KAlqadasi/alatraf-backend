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

    public async Task<Result<IReadOnlyList<UserListItemDto>>> GetUsersAsync(string? searchBy,bool? isActive, CancellationToken ct)
    {
        IQueryable<AppUser> query = _db.Set<AppUser>();

        if (!string.IsNullOrWhiteSpace(searchBy))
        {
            var pattern = $"%{searchBy.Trim().ToLower()}%";

            query = query.Where(p =>
                EF.Functions.Like(p.UserName!.ToLower(), pattern)
                ||
                EF.Functions.Like(p.Person.FullName.ToLower(), pattern)
                || 
                EF.Functions.Like(p.Person.Phone, pattern)
                );
        }
        
        if (isActive.HasValue)
        {
            query = query.Where(p=> p.IsActive == isActive);
        }


        return await query
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

    public async Task<Result<Updated>> UpsertUserRolesAsync(
    string userId,
    IReadOnlyCollection<string> roleIds,
    CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return MyIdentityErrors.UserNotFound;

        if (roleIds is null || roleIds.Count == 0)
            return MyIdentityErrors.RoleNotFound;

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return MyIdentityErrors.UserNotFound;

        // Resolve roles by IDs
        var roles = new List<IdentityRole>();

        foreach (var roleId in roleIds.Distinct())
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role is null)
                return MyIdentityErrors.RoleNotFound;

            roles.Add(role);
        }

        // Identity works with role NAMES, not IDs
        var desiredRoleNames = roles
            .Select(r => r.Name!)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        // Get current user roles
        var existingRoleNames = await _userManager.GetRolesAsync(user);

        // Roles to add (desired - existing)
        var rolesToAdd = desiredRoleNames
            .Except(existingRoleNames, StringComparer.OrdinalIgnoreCase)
            .ToList();

        // Roles to remove (existing - desired)
        var rolesToRemove = existingRoleNames
            .Except(desiredRoleNames, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (rolesToAdd.Count > 0)
        {
            var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
            if (!addResult.Succeeded)
                return MyIdentityErrors.FaliedToAssignRoleToUser;
        }

        if (rolesToRemove.Count > 0)
        {
            var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            if (!removeResult.Succeeded)
                return MyIdentityErrors.FaliedToRemoveRoleFromUser;
        }

        return Result.Updated;
    }

    public async Task<Result<Updated>> ActivateRolePermissionsAsync(
        string roleId,
        IReadOnlyCollection<int> permissionIds,
        CancellationToken ct = default)
    {
        return await SetRolePermissionsStatusAsync(roleId, permissionIds, true, ct);
    }

    public async Task<Result<Updated>> DeactivateRolePermissionsAsync(
        string roleId,
        IReadOnlyCollection<int> permissionIds,
        CancellationToken ct = default)
    {
        return await SetRolePermissionsStatusAsync(roleId, permissionIds, false, ct);
    }

    private async Task<Result<Updated>> SetRolePermissionsStatusAsync(
        string roleId,
        IReadOnlyCollection<int> permissionIds,
        bool isActive,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(roleId))
            return MyIdentityErrors.RoleNotFound;

        if (permissionIds is null || permissionIds.Count == 0)
            return Result.Updated; // Idempotent - no permissions to process

        // Get all existing role permissions for this role
        var existingRolePermissions = await _db.RolePermissions
            .Where(rp => rp.RoleId == roleId && permissionIds.Contains(rp.PermissionId))
            .ToListAsync(ct);

        // Check if all requested permissions exist in the role
        var requestedPermissionsSet = permissionIds.ToHashSet();
        var existingPermissionsSet = existingRolePermissions.Select(rp => rp.PermissionId).ToHashSet();
        
        // Find permissions that are requested but not in the role
        var missingPermissions = requestedPermissionsSet.Except(existingPermissionsSet).ToList();
        
        if (missingPermissions.Count > 0)
        {
            // Check which missing permissions actually exist in the system
            var existingSystemPermissions = await _db.Set<ApplicationPermission>()
                .Where(p => missingPermissions.Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync(ct);
            
            if (existingSystemPermissions.Count > 0)
            {
                // Some permissions exist in system but not in role
                return MyIdentityErrors.PermissionsNotInRole;
            }
        }

        var permissionsToUpdate = existingRolePermissions
            .Where(rp => rp.IsActive != isActive)
            .ToList();

        if (permissionsToUpdate.Count == 0)
            return Result.Updated; // Idempotent - nothing to change

        foreach (var rolePermission in permissionsToUpdate)
        {
            rolePermission.IsActive = isActive;
        }

        _db.RolePermissions.UpdateRange(permissionsToUpdate);

     
            await _db.SaveChangesAsync(ct);
            return Result.Updated;
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

   public async Task<Result<Updated>> UpsertPermissionsForUserAsync(
    string userId,
    IReadOnlyCollection<int> permissionIds,
    CancellationToken ct)
    {
        var desiredPermissions = permissionIds?.Distinct().ToHashSet()
            ?? new HashSet<int>();

        var existingOverrides = await _db.Set<UserPermissionOverride>()
            .Where(x => x.UserId == userId)
            .ToListAsync(ct);

        var existingMap = existingOverrides
            .ToDictionary(x => x.PermissionId);

        // 1. Grant permissions that should exist
        foreach (var permissionId in desiredPermissions)
        {
            if (existingMap.TryGetValue(permissionId, out var existing))
            {
                if (existing.Effect != Effect.Grant)
                    existing.Effect = Effect.Grant;
            }
            else
            {
                _db.Add(new UserPermissionOverride
                {
                    UserId = userId,
                    PermissionId = permissionId,
                    Effect = Effect.Grant
                });
            }
        }

        // 2. Deny permissions that should NOT exist
        foreach (var existing in existingOverrides)
        {
            if (!desiredPermissions.Contains(existing.PermissionId) &&
                existing.Effect != Effect.Deny)
            {
                existing.Effect = Effect.Deny;
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
            where ur.UserId == userId && rp.IsActive
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
    
    public async Task<Result<IReadOnlyList<PermissionDto>>> GetAllPermissionsAsync(string? search, CancellationToken ct)
    {
        IQueryable<ApplicationPermission> query = _db.Set<ApplicationPermission>();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search.Trim().ToLower()}%";

            query = query.Where(p =>
                EF.Functions.Like(p.Name.ToLower(), pattern));
        }

        var permissions = await query
            .Select(p => new PermissionDto
            {
                PermissionId = p.Id,
                Name = p.Name,
                Description = p.Description
            })
            .ToListAsync(ct);

        return permissions;
    }
    public async Task<bool> IsUserNameExistsAsync(string userName, CancellationToken ct = default)
    {
        var user = await _userManager.FindByNameAsync(userName.Trim());
        return user != null;
    }
}