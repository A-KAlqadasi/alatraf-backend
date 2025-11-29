using System.Security.Claims;
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Identity;
using AlatrafClinic.Application.Features.Identity.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Identity;
using AlatrafClinic.Infrastructure.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Infrastructure.Identity;

public class IdentityService(
    UserManager<AppUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IUserClaimsPrincipalFactory<AppUser> userClaimsPrincipalFactory,
    IAuthorizationService authorizationService,
    ApplicationDbContext context)
    : IIdentityService
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private readonly IUserClaimsPrincipalFactory<AppUser> _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService = authorizationService;
    private readonly ApplicationDbContext _context = context;

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user != null && await _userManager.IsInRoleAsync(user, role);
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

    public async Task<Result<AppUserDto>> AuthenticateAsync(string userName, string password)
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
        var permissions = await GetPermissionsForUserAsync(user, roles);

        var dto = new AppUserDto(
            user.Id,
            user.UserName!,
            roles,
            permissions);

        return dto;
    }

    public async Task<Result<AppUserDto>> GetUserByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return Error.NotFound(
                "User_Not_Found",
                $"User with id '{userId}' not found");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var permissions = await GetPermissionsForUserAsync(user, roles);

        var dto = new AppUserDto(
            user.Id,
            user.UserName!,
            roles,
            permissions);

        return dto;
    }

    public async Task<string?> GetUserNameAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user?.UserName;
    }

    public async Task<Result<RefreshToken>> GetRefreshTokenAsync(string refreshToken, string userId)
    {
        var token = await _context.RefreshTokens
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

    // ------------------------
    // NEW: Permissions for roles
    // ------------------------

    public async Task<Result<bool>> AddPermissionToRoleAsync(string roleName, string permissionName, CancellationToken ct = default)
    {
        var role = await _roleManager.FindByNameAsync(roleName);

        if (role is null)
        {
            return Error.NotFound(
                "Role_Not_Found",
                $"Role '{roleName}' not found.");
        }

        var permission = await _context.Permissions
            .FirstOrDefaultAsync(p => p.Name == permissionName, ct);

        if (permission is null)
        {
            permission = new ApplicationPermission
            {
                Name = permissionName
            };

            _context.Permissions.Add(permission);
            await _context.SaveChangesAsync(ct);
        }

        var exists = await _context.RolePermissions
            .AnyAsync(rp => rp.RoleId == role.Id && rp.PermissionId == permission.Id, ct);

        if (exists)
        {
            // idempotent
            return true;
        }

        _context.RolePermissions.Add(new RolePermission
        {
            RoleId = role.Id,
            PermissionId = permission.Id
        });

        await _context.SaveChangesAsync(ct);

        return true;
    }

    public async Task<Result<bool>> RemovePermissionFromRoleAsync(string roleName, string permissionName, CancellationToken ct = default)
    {
        var role = await _roleManager.FindByNameAsync(roleName);

        if (role is null)
        {
            return Error.NotFound(
                "Role_Not_Found",
                $"Role '{roleName}' not found.");
        }

        var permission = await _context.Permissions
            .FirstOrDefaultAsync(p => p.Name == permissionName, ct);

        if (permission is null)
        {
            // Nothing to remove – treat as success (idempotent)
            return true;
        }

        var existing = await _context.RolePermissions
            .FirstOrDefaultAsync(rp => rp.RoleId == role.Id && rp.PermissionId == permission.Id, ct);

        if (existing is null)
        {
            return true;
        }

        _context.RolePermissions.Remove(existing);
        await _context.SaveChangesAsync(ct);

        return true;
    }

    // ------------------------
    // NEW: Permissions for users
    // ------------------------

    public async Task<Result<bool>> AddPermissionToUserAsync(string userId, string permissionName, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return Error.NotFound(
                "User_Not_Found",
                $"User with id '{userId}' not found.");
        }

        var permission = await _context.Permissions
            .FirstOrDefaultAsync(p => p.Name == permissionName, ct);

        if (permission is null)
        {
            permission = new ApplicationPermission
            {
                Name = permissionName
            };

            _context.Permissions.Add(permission);
            await _context.SaveChangesAsync(ct);
        }

        var exists = await _context.UserPermissions
            .AnyAsync(up => up.UserId == user.Id && up.PermissionId == permission.Id, ct);

        if (exists)
        {
            return true;
        }

        _context.UserPermissions.Add(new UserPermission
        {
            UserId = user.Id,
            PermissionId = permission.Id
        });

        await _context.SaveChangesAsync(ct);

        return true;
    }

    public async Task<Result<bool>> RemovePermissionFromUserAsync(string userId, string permissionName, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return Error.NotFound(
                "User_Not_Found",
                $"User with id '{userId}' not found.");
        }

        var permission = await _context.Permissions
            .FirstOrDefaultAsync(p => p.Name == permissionName, ct);

        if (permission is null)
        {
            return true;
        }

        var existing = await _context.UserPermissions
            .FirstOrDefaultAsync(up => up.UserId == user.Id && up.PermissionId == permission.Id, ct);

        if (existing is null)
        {
            return true;
        }

        _context.UserPermissions.Remove(existing);
        await _context.SaveChangesAsync(ct);

        return true;
    }

    // ------------------------
    // Helpers
    // ------------------------

    private async Task<IList<string>> GetPermissionsForUserAsync(AppUser user, IList<string> roleNames)
    {
        // Get role IDs
        var roleIds = await _roleManager.Roles
            .Where(r => roleNames.Contains(r.Name!))
            .Select(r => r.Id)
            .ToListAsync();

        // Permissions from roles
        var rolePermissions = await _context.RolePermissions
            .Where(rp => roleIds.Contains(rp.RoleId))
            .Select(rp => rp.Permission.Name)
            .ToListAsync();

        // Direct user permissions
        var userPermissions = await _context.UserPermissions
            .Where(up => up.UserId == user.Id)
            .Select(up => up.Permission.Name)
            .ToListAsync();

        return rolePermissions
            .Concat(userPermissions)
            .Distinct()
            .ToList();
    }
}
