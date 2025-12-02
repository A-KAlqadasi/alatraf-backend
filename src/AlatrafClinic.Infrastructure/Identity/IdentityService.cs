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

        var permission = await _dbContext.Permissions
            .FirstOrDefaultAsync(p => p.Name == permissionName, ct);

        if (permission is null)
        {
            permission = new ApplicationPermission
            {
                Name = permissionName
            };

            _dbContext.Permissions.Add(permission);
            await _dbContext.SaveChangesAsync(ct);
        }

        var exists = await _dbContext.RolePermissions
            .AnyAsync(rp => rp.RoleId == role.Id && rp.PermissionId == permission.Id, ct);

        if (exists)
        {
            return true;
        }

        _dbContext.RolePermissions.Add(new RolePermission
        {
            RoleId = role.Id,
            PermissionId = permission.Id
        });

        await _dbContext.SaveChangesAsync(ct);

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

        var permission = await _dbContext.Permissions
            .FirstOrDefaultAsync(p => p.Name == permissionName, ct);

        if (permission is null)
        {
            // Nothing to remove – treat as success (idempotent)
            return true;
        }

        var existing = await _dbContext.RolePermissions
            .FirstOrDefaultAsync(rp => rp.RoleId == role.Id && rp.PermissionId == permission.Id, ct);

        if (existing is null)
        {
            return true;
        }

        _dbContext.RolePermissions.Remove(existing);
        await _dbContext.SaveChangesAsync(ct);

        return true;
    }

    public async Task<Result<bool>> AddPermissionToUserAsync(string userId, string permissionName, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return Error.NotFound(
                "User_Not_Found",
                $"User with id '{userId}' not found.");
        }

        var permission = await _dbContext.Permissions
            .FirstOrDefaultAsync(p => p.Name == permissionName, ct);

        if (permission is null)
        {
            permission = new ApplicationPermission
            {
                Name = permissionName
            };

            _dbContext.Permissions.Add(permission);
            await _dbContext.SaveChangesAsync(ct);
        }

        var exists = await _dbContext.UserPermissions
            .AnyAsync(up => up.UserId == user.Id && up.PermissionId == permission.Id, ct);

        if (exists)
        {
            return true;
        }

        _dbContext.UserPermissions.Add(new UserPermission
        {
            UserId = user.Id,
            PermissionId = permission.Id
        });

        await _dbContext.SaveChangesAsync(ct);

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

        var permission = await _dbContext.Permissions
            .FirstOrDefaultAsync(p => p.Name == permissionName, ct);

        if (permission is null)
        {
            return true;
        }

        var existing = await _dbContext.UserPermissions
            .FirstOrDefaultAsync(up => up.UserId == user.Id && up.PermissionId == permission.Id, ct);

        if (existing is null)
        {
            return true;
        }

        _dbContext.UserPermissions.Remove(existing);
        await _dbContext.SaveChangesAsync(ct);

        return true;
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

        foreach (var permission in permissions)
        {
            var permissionResult = await AddPermissionToUserAsync(user.Id, permission);
            if (!permissionResult.IsSuccess)
            {
                return permissionResult.Errors;
            }
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
        var usersQuery =  _userManager.Users
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