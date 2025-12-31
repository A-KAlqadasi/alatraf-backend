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
        var permissions = await GetEffectivePermissionsAsync(user.Id, CancellationToken.None);

        var dto = new UserDetailsDto
        {
            UserId = user.Id,
            Username = user.UserName!,
            IsActive = user.IsActive,
            Roles = roles.ToList(),
            PermissionOverrides = permissions.Value
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

    public async Task<Result<string>> CreateUserAsync(CreateUserRequest request, CancellationToken ct)
    {
        var user = new AppUser
        {
            PersonId = request.PersonId,
            IsActive = request.IsActive
        };

        var result = await _userManager.CreateAsync(user);
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

    public async Task<Result<Updated>> ResetUserPasswordAsync(ResetPasswordRequest request, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);

        if (user is null)
            return MyIdentityErrors.UserNotFound;

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);

        if (!result.Succeeded)
            return MyIdentityErrors.FailToResetPassword;
        
        return Result.Updated;
    }

    public async Task<Result<UserDetailsDto>> GetUserByIdAsync(string userId, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return MyIdentityErrors.UserNotFound;


        var roles = await _userManager.GetRolesAsync(user);

        var overrides = await _db.UserPermissionOverrides
            .Where(x => x.UserId == userId)
            .Select(x => new UserPermissionOverrideDto
            {
                PermissionId = x.PermissionId,
                PermissionName = x.Permission.Name,
                Effect = x.Effect.ToString()
            })
            .ToListAsync(ct);

        return new UserDetailsDto
        {
            UserId = user.Id,
            Username = user.UserName!,
            IsActive = user.IsActive,
            Roles = roles.ToList(),
            PermissionOverrides = [""] //overrides
        };
    }

    public async Task<Result<IReadOnlyList<UserListItemDto>>> GetUsersAsync(CancellationToken ct)
    {
        return await _db.Users
            .Select(u => new UserListItemDto
            {
                UserId = u.Id,
                Username = u.UserName!,
                IsActive = u.IsActive
            })
            .ToListAsync(ct);
    }

    public async Task<Result<Updated>> AssignRoleToUserAsync(AssignRoleRequest request, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);

        if (user is null)
            return MyIdentityErrors.UserNotFound;

        var role = await _roleManager.FindByIdAsync(request.RoleId);

        if (role is null)
            return MyIdentityErrors.RoleNotFound;

        await _userManager.AddToRoleAsync(user, role.Name!);

        return Result.Updated;
    }

    public async Task<Result<Deleted>> RemoveRoleFromUserAsync(AssignRoleRequest request, CancellationToken ct)
    {
       var user = await _userManager.FindByIdAsync(request.UserId);

        if (user is null)
            return MyIdentityErrors.UserNotFound;

        var role = await _roleManager.FindByIdAsync(request.RoleId);

        if (role is null)
            return MyIdentityErrors.RoleNotFound;

        await _userManager.RemoveFromRoleAsync(user, role.Name!);

        return Result.Deleted;
    }

    // =========================
    // Role Management
    // =========================
    public async Task<Result<string>> CreateRoleAsync(CreateRoleRequest request, CancellationToken ct)
    {
        var role = new IdentityRole(request.Name);
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

    public async Task<Result<Updated>> AssignPermissionToRoleAsync(RolePermissionRequest request, CancellationToken ct)
    {
        var exists = await _db.Set<RolePermission>()
            .AnyAsync(x => x.RoleId == request.RoleId && x.PermissionId == request.PermissionId, ct);

        if (exists) return Result.Updated;

        _db.Add(new RolePermission
        {
            RoleId = request.RoleId,
            PermissionId = request.PermissionId
        });

        await _db.SaveChangesAsync(ct);
        return Result.Updated;
    }

    public async Task<Result<Deleted>> RemovePermissionFromRoleAsync(RolePermissionRequest request, CancellationToken ct)
    {
        var rp = await _db.Set<RolePermission>()
            .SingleOrDefaultAsync(x =>
                x.RoleId == request.RoleId &&
                x.PermissionId == request.PermissionId, ct);

        if (rp is null) return Result.Deleted;

        _db.Remove(rp);
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
    public async Task<Result<Updated>> GrantPermissionToUserAsync(UserPermissionOverrideRequest request, CancellationToken ct)
        => await SetUserPermissionOverride(request, Effect.Grant, ct);

    public async Task<Result<Updated>> DenyPermissionToUserAsync(UserPermissionOverrideRequest request, CancellationToken ct)
        => await SetUserPermissionOverride(request, Effect.Deny, ct);

    public async Task<Result<Deleted>> RemoveUserPermissionOverrideAsync(UserPermissionOverrideRequest request, CancellationToken ct)
    {
        var existing = await _db.Set<UserPermissionOverride>()
            .SingleOrDefaultAsync(x =>
                x.UserId == request.UserId &&
                x.PermissionId == request.PermissionId, ct);

        if (existing is null) return Result.Deleted;

        _db.Remove(existing);
        await _db.SaveChangesAsync(ct);
        return Result.Deleted;
    }

    private async Task<Result<Updated>> SetUserPermissionOverride(
        UserPermissionOverrideRequest request,
        Effect efect,
        CancellationToken ct)
    {
        var existing = await _db.Set<UserPermissionOverride>()
            .SingleOrDefaultAsync(x =>
                x.UserId == request.UserId &&
                x.PermissionId == request.PermissionId, ct);

        if (existing is null)
        {
            _db.Add(new UserPermissionOverride
            {
                UserId = request.UserId,
                PermissionId = request.PermissionId,
                Effect = efect
            });
        }
        else
        {
            existing.Effect = efect;
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
            .SingleAsync(p => p.Name == permissionName, ct);

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

    public async Task<Result<IReadOnlyList<string>>> GetEffectivePermissionsAsync(
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
}