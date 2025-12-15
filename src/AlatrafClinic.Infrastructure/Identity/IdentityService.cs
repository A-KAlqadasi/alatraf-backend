using System.Security.Claims;

using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Identity;
using AlatrafClinic.Application.Features.Identity.Dtos;
using AlatrafClinic.Application.Features.People.Mappers;
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
    AlatrafClinicDbContext dbContext)
    : IIdentityService
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private readonly IUserClaimsPrincipalFactory<AppUser> _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService = authorizationService;
    private readonly AlatrafClinicDbContext _dbContext = dbContext;

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
            user.IsActive,
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
        var token = await _dbContext.RefreshTokens
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

    public async Task<Result<Success>> AddPermissionsToRoleAsync(
    string roleName,
    IList<string> permissionNames,
    CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            return Error.Validation("RoleName_Required", "Role name is required.");
        }

        if (permissionNames is null || permissionNames.Count == 0)
        {
            return Error.Validation("Permissions_Required", "At least one permission is required.");
        }

        // Normalize + de-duplicate (case-insensitive)
        var normalizedNames = permissionNames
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(p => p.Trim())
            .Where(p => p.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (normalizedNames.Count == 0)
        {
            return Error.Validation("Permissions_Required", "At least one valid permission is required.");
        }

        // Load role
        var role = await _roleManager.FindByNameAsync(roleName);
        if (role is null)
        {
            return Error.NotFound("Role_Not_Found", $"Role '{roleName}' not found.");
        }

        // 1) Load existing permissions by name
        var existingPermissions = await _dbContext.Permissions
            .Where(p => normalizedNames.Contains(p.Name))
            .ToListAsync(ct);

        var permissionByName = existingPermissions
            .GroupBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

        // 2) Create missing permissions
        var missingNames = normalizedNames
            .Where(n => !permissionByName.ContainsKey(n))
            .ToList();

        if (missingNames.Count > 0)
        {
            var newPermissions = missingNames.Select(n => new ApplicationPermission
            {
                Name = n
            }).ToList();

            _dbContext.Permissions.AddRange(newPermissions);
            await _dbContext.SaveChangesAsync(ct);

            foreach (var p in newPermissions)
            {
                permissionByName[p.Name] = p;
            }
        }

        var permissionIds = permissionByName.Values
            .Select(p => p.Id)
            .Distinct()
            .ToList();

        // 3) Load existing role-permission links
        var existingRolePermissionIds = await _dbContext.RolePermissions
            .Where(rp => rp.RoleId == role.Id && permissionIds.Contains(rp.PermissionId))
            .Select(rp => rp.PermissionId)
            .ToListAsync(ct);

        var existingSet = existingRolePermissionIds.ToHashSet();

        // 4) Insert missing links
        var toInsert = permissionIds
            .Where(pid => !existingSet.Contains(pid))
            .Select(pid => new RolePermission
            {
                RoleId = role.Id,      // string
                PermissionId = pid     // int
            })
            .ToList();

        if (toInsert.Count > 0)
        {
            _dbContext.RolePermissions.AddRange(toInsert);
            await _dbContext.SaveChangesAsync(ct);
        }

        return Result.Success;
    }

    public async Task<Result<Success>> RemovePermissionsFromRoleAsync(
    string roleName,
    IList<string> permissionNames,
    CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            return Error.Validation("RoleName_Required", "Role name is required.");
        }

        if (permissionNames is null || permissionNames.Count == 0)
        {
            return Error.Validation("Permissions_Required", "At least one permission is required.");
        }

        // Normalize + de-duplicate (case-insensitive)
        var normalizedNames = permissionNames
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(p => p.Trim())
            .Where(p => p.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (normalizedNames.Count == 0)
        {
            return Error.Validation("Permissions_Required", "At least one valid permission is required.");
        }

        // Load role
        var role = await _roleManager.FindByNameAsync(roleName);
        if (role is null)
        {
            return Error.NotFound("Role_Not_Found", $"Role '{roleName}' not found.");
        }

        // 1) Load permissions by name (ignore missing permissions)
        var permissionIds = await _dbContext.Permissions
            .Where(p => normalizedNames.Contains(p.Name))
            .Select(p => p.Id)
            .ToListAsync(ct);

        if (permissionIds.Count == 0)
        {
            return Result.Success;
        }

        // 2) Load matching role-permission links
        var rolePermissions = await _dbContext.RolePermissions
            .Where(rp => rp.RoleId == role.Id && permissionIds.Contains(rp.PermissionId))
            .ToListAsync(ct);

        if (rolePermissions.Count == 0)
        {
            return Result.Success;
        }

        // 3) Remove in bulk
        _dbContext.RolePermissions.RemoveRange(rolePermissions);
        await _dbContext.SaveChangesAsync(ct);

        return Result.Success;
    }
    public async Task<Result<Success>> AddPermissionsToUserAsync(
    string userId,
    IList<string> permissionNames,
    CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Error.Validation("UserId_Required", "UserId is required.");
        }

        if (permissionNames is null || permissionNames.Count == 0)
        {
            return Error.Validation("Permissions_Required", "At least one permission is required.");
        }

        // Normalize + de-duplicate (case-insensitive)
        var normalizedNames = permissionNames
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(p => p.Trim())
            .Where(p => p.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (normalizedNames.Count == 0)
        {
            return Error.Validation("Permissions_Required", "At least one valid permission is required.");
        }

        // Load user (AppUser) using Identity
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return Error.NotFound("User_Not_Found", $"User with id '{userId}' not found.");
        }

        // 1) Load existing permissions by Name
        var existingPermissions = await _dbContext.Permissions
            .Where(p => normalizedNames.Contains(p.Name))
            .ToListAsync(ct);

        var permissionByName = existingPermissions
            .GroupBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

        // 2) Create missing permissions (ApplicationPermission)
        var missingNames = normalizedNames
            .Where(n => !permissionByName.ContainsKey(n))
            .ToList();

        if (missingNames.Count > 0)
        {
            var newPermissions = missingNames.Select(n => new ApplicationPermission
            {
                Name = n
                // Description left null intentionally
            }).ToList();

            _dbContext.Permissions.AddRange(newPermissions);

            // Save so IDs are generated (PermissionId is int)
            await _dbContext.SaveChangesAsync(ct);

            // Add newly created permissions to lookup (tracked entities now have Id)
            foreach (var p in newPermissions)
            {
                permissionByName[p.Name] = p;
            }
        }

        var permissionIds = permissionByName.Values
            .Select(p => p.Id)
            .Distinct()
            .ToList();

        // 3) Load existing user-permission links for those permissions
        var existingUserPermissionIds = await _dbContext.UserPermissions
            .Where(up => up.UserId == user.Id && permissionIds.Contains(up.PermissionId))
            .Select(up => up.PermissionId)
            .ToListAsync(ct);

        var existingSet = existingUserPermissionIds.ToHashSet();

        // 4) Insert missing join rows (UserPermission)
        var toInsert = permissionIds
            .Where(pid => !existingSet.Contains(pid))
            .Select(pid => new UserPermission
            {
                UserId = user.Id,          // string
                PermissionId = pid         // int
            })
            .ToList();

        if (toInsert.Count > 0)
        {
            _dbContext.UserPermissions.AddRange(toInsert);
            await _dbContext.SaveChangesAsync(ct);
        }

        return Result.Success;
    }


    public async Task<Result<Success>> RemovePermissionsFromUserAsync(
    string userId,
    IList<string> permissionNames,
    CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Error.Validation("UserId_Required", "UserId is required.");
        }

        if (permissionNames is null || permissionNames.Count == 0)
        {
            return Error.Validation("Permissions_Required", "At least one permission is required.");
        }

        // Normalize + de-duplicate (case-insensitive)
        var normalizedNames = permissionNames
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(p => p.Trim())
            .Where(p => p.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (normalizedNames.Count == 0)
        {
            return Error.Validation("Permissions_Required", "At least one valid permission is required.");
        }

        // Load user
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return Error.NotFound("User_Not_Found", $"User with id '{userId}' not found.");
        }

        // 1) Load permissions by name (ignore missing permissions)
        var permissions = await _dbContext.Permissions
            .Where(p => normalizedNames.Contains(p.Name))
            .Select(p => new { p.Id })
            .ToListAsync(ct);

        if (permissions.Count == 0)
        {
            // Nothing to remove
            return Result.Success;
        }

        var permissionIds = permissions
            .Select(p => p.Id)
            .ToList();

        // 2) Load matching user-permission links
        var userPermissions = await _dbContext.UserPermissions
            .Where(up => up.UserId == user.Id && permissionIds.Contains(up.PermissionId))
            .ToListAsync(ct);

        if (userPermissions.Count == 0)
        {
            // User does not have any of these permissions
            return Result.Success;
        }

        // 3) Remove in bulk
        _dbContext.UserPermissions.RemoveRange(userPermissions);
        await _dbContext.SaveChangesAsync(ct);

        return Result.Success;
    }


    private async Task<IList<string>> GetPermissionsForUserAsync(AppUser user, IList<string> roleNames)
    {
        var roleIds = await _roleManager.Roles
            .Where(r => roleNames.Contains(r.Name!))
            .Select(r => r.Id)
            .ToListAsync();

        var rolePermissions = await _dbContext.RolePermissions
            .Where(rp => roleIds.Contains(rp.RoleId))
            .Select(rp => rp.Permission.Name)
            .ToListAsync();

        var userPermissions = await _dbContext.UserPermissions
            .Where(up => up.UserId == user.Id)
            .Select(up => up.Permission.Name)
            .ToListAsync();

        return rolePermissions
            .Concat(userPermissions)
            .Distinct()
            .ToList();
    }

    public Task<bool> IsUserNameExistsAsync(string userName)
    {
        return _userManager.Users.AnyAsync(u => u.UserName == userName);
    }

    public async Task<Result<AppUserDto>> CreateUserAsync(int pesonId, string userName, string password, bool isActive, IList<string> roles, IList<string> permissions)
    {
        var isPersonExists = await _dbContext.People.AnyAsync(p => p.Id == pesonId);
        if (!isPersonExists)
        {
            return Error.NotFound(
                "Person_Not_Found",
                $"Person with id '{pesonId}' not found.");
        }

        var user = new AppUser
        {
            UserName = userName,
            NormalizedUserName = _userManager.NormalizeName(userName),
            PersonId = pesonId,
            IsActive = isActive,
            EmailConfirmed = true
        };
        var result = await _userManager.CreateAsync(user, password);
        
        if (!result.Succeeded)
        {
            return Error.Conflict(
                "User_Creation_Failed",
                string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        foreach (var role in roles)
        {
            var roleResult = await _userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
            {
                return Error.Conflict(
                    "Add_Role_Failed",
                    string.Join("; ", roleResult.Errors.Select(e => e.Description)));
            }
        }

        var addPermissionsResult = await AddPermissionsToUserAsync(
            user.Id,
            permissions);
        if (addPermissionsResult.IsError)
        {
            return addPermissionsResult.Errors;
        }

        var dto = new AppUserDto(
            user.Id,
            user.UserName!,
            user.IsActive,
            roles,
            permissions);
        return dto;
    }

    public async Task<Result<bool>> ChangeUserNameAndPasswordAsync(string userId, string newUsername, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Error.NotFound(
                "User_Not_Found",
                $"User with id '{userId}' not found.");
        }
        
        user.UserName = newUsername;
        user.NormalizedUserName = _userManager.NormalizeName(newUsername);

        var usernameResult = await _userManager.UpdateAsync(user);
        if (!usernameResult.Succeeded)
        {
            return Error.Conflict(
                "Update_Username_Failed",
                string.Join("; ", usernameResult.Errors.Select(e => e.Description)));
        }
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var passwordResult = await _userManager.ResetPasswordAsync(user, token, newPassword);
        
        if (!passwordResult.Succeeded)
        {
            return Error.Conflict(
                "Update_Password_Failed",
                string.Join("; ", passwordResult.Errors.Select(e => e.Description)));
        }

        return true;
    }

    public async Task<Result<bool>> ChangeUserActivationAsync(string userId, bool isActive)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Error.NotFound(
                "User_Not_Found",
                $"User with id '{userId}' not found.");
        }
        user.IsActive = isActive;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return Error.Conflict(
                "Update_User_Activation_Failed",
                string.Join("; ", result.Errors.Select(e => e.Description)));
        }
        return true;
    }

    public async Task<IQueryable<UserDto>> GetUsersAsync()
    {
        var usersQuery = _userManager.Users
            .SelectMany(u => _dbContext.People
                .Where(p => p.Id == u.PersonId)
                .Select(p => new { User = u, Person = p }))
            .Select(up => new UserDto
            {
                UserId = up.User.Id,
                PersonId = up.Person.Id,
                Person = up.Person.ToDto(),
                IsActive = up.User.IsActive,
                UserName = up.User.UserName,
                Roles = _userManager.GetRolesAsync(up.User).Result.ToList(),
                Permissions = GetPermissionsForUserAsync(up.User, _userManager.GetRolesAsync(up.User).Result).Result.ToList()
            });

        return usersQuery;
    }

    public async Task<Result<UserDto>> GetUserByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return Error.NotFound(
                "User_Not_Found",
                $"User with id '{userId}' not found");
        }

        var person = await _dbContext.People
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == user.PersonId);
        var roles = await _userManager.GetRolesAsync(user);
        var permissions = await GetPermissionsForUserAsync(user, roles);


        var dto = new UserDto
        {
            UserId = user.Id,
            PersonId = user.PersonId,
            Person = person != null ? person.ToDto() : null,
            IsActive = user.IsActive,
            UserName = user.UserName,
            Roles = roles.ToList(),
            Permissions = permissions.ToList()
        };
        
        return dto;
    }

    public async Task<Result<bool>> ChangeUserNameAndPasswordAsync(
    string userId,
    string newUsername,
    string oldPassword,
    string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Error.NotFound(
                "User_Not_Found",
                $"User with id '{userId}' not found.");
        }

        if (user.UserName != newUsername)
        {
            user.UserName = newUsername;
            user.NormalizedUserName = _userManager.NormalizeName(newUsername);

            var usernameResult = await _userManager.UpdateAsync(user);
            if (!usernameResult.Succeeded)
            {
                return Error.Conflict(
                    "Update_Username_Failed",
                    string.Join("; ", usernameResult.Errors.Select(e => e.Description)));
            }
        }

        var passwordResult = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);

        if (!passwordResult.Succeeded)
        {
            return Error.Conflict(
                "Update_Password_Failed",
                string.Join("; ", passwordResult.Errors.Select(e => e.Description)));
        }

        return true;
    }

}