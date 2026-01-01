
using System.Security.Claims;

using AlatrafClinic.Api.Requests.Identity;
using AlatrafClinic.Application.Features.Identity;
using AlatrafClinic.Application.Features.Identity.Commands.ActivateUser;
using AlatrafClinic.Application.Features.Identity.Commands.AssignPermissionsToRole;
using AlatrafClinic.Application.Features.Identity.Commands.AssignUserToRole;
using AlatrafClinic.Application.Features.Identity.Commands.ChangeUserCredentials;
using AlatrafClinic.Application.Features.Identity.Commands.CreateRole;
using AlatrafClinic.Application.Features.Identity.Commands.CreateUser;
using AlatrafClinic.Application.Features.Identity.Commands.DeleteRole;
using AlatrafClinic.Application.Features.Identity.Commands.DenyPermissionsToUser;
using AlatrafClinic.Application.Features.Identity.Commands.GrantPermissionsToUser;
using AlatrafClinic.Application.Features.Identity.Commands.RemovePermissionsFromRole;
using AlatrafClinic.Application.Features.Identity.Commands.RemoveRoleFromUser;
using AlatrafClinic.Application.Features.Identity.Commands.RemoveUserPermissionOverrides;
using AlatrafClinic.Application.Features.Identity.Commands.ResetUserPassword;
using AlatrafClinic.Application.Features.Identity.Dtos;
using AlatrafClinic.Application.Features.Identity.Queries.GenerateTokens;
using AlatrafClinic.Application.Features.Identity.Queries.GetAllPermissions;
using AlatrafClinic.Application.Features.Identity.Queries.GetEffectiveUserPermissions;
using AlatrafClinic.Application.Features.Identity.Queries.GetRoles;
using AlatrafClinic.Application.Features.Identity.Queries.GetUser;
using AlatrafClinic.Application.Features.Identity.Queries.GetUsers;
using AlatrafClinic.Application.Features.Identity.Queries.RefreshTokens;

using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlatrafClinic.Api.Controllers;

[Route("identity")]
[ApiVersionNeutral]
public sealed class IdentityController(ISender sender) : ApiController
{
    [HttpPost("token/generate")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Generates an access and refresh token for a valid user.")]
    [EndpointDescription("Authenticates a user using provided credentials and returns a JWT token pair.")]
    [EndpointName("GenerateToken")]
    public async Task<IActionResult> GenerateToken([FromBody] GenerateTokenQuery request, CancellationToken ct)
    {
        var result = await sender.Send(request, ct);
        return result.Match(
            response => Ok(response),
            Problem);
    }

    [HttpPost("token/refresh-token")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Refreshes access token using a valid refresh token.")]
    [EndpointDescription("Exchanges an expired access token and a valid refresh token for a new token pair.")]
    [EndpointName("RefreshToken")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenQuery request, CancellationToken ct)
    {
        var result = await sender.Send(request, ct);
        return result.Match(
            response => Ok(response),
            Problem);
    }

    [HttpGet("current-user/claims")]
    [Authorize]
    [ProducesResponseType(typeof(UserDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Gets the current authenticated user's info.")]
    [EndpointDescription("Returns user information for the currently authenticated user based on the access token.")]
    [EndpointName("GetCurrentUserClaims")]
    public async Task<IActionResult> GetCurrentUserInfo(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var result = await sender.Send(new GetUserByIdQuery(userId), ct);

        return result.Match(
            response => Ok(response),
            Problem);
    }
    [HttpPost("users")]
    [EndpointSummary("Creates a new user.")]
    [EndpointName("CreateUser")]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserRequest request,
        CancellationToken ct)
    {
        var command = new CreateUserCommand(
            request.PersonId,
            request.UserName,
            request.Password,
            request.IsActive);

        var result = await sender.Send(command, ct);
        return result.Match(Ok, Problem);
    }

    [HttpPatch("users/{userId}/activation")]
    [EndpointSummary("Activates or deactivates a user.")]
    [EndpointName("ActivateUser")]
    public async Task<IActionResult> ActivateUser(
        string userId,
        [FromBody] ActivateUserRequest request,
        CancellationToken ct)
    {
        var command = new ActivateUserCommand(userId, request.IsActive);
        var result = await sender.Send(command, ct);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPatch("users/{userId}/password/reset")]
    [EndpointSummary("Resets a user's password.")]
    [EndpointName("ResetUserPassword")]
    public async Task<IActionResult> ResetPassword(
        string userId,
        [FromBody] ResetPasswordRequest request,
        CancellationToken ct)
    {
        var command = new ResetUserPasswordCommand(userId, request.NewPassword);
        var result = await sender.Send(command, ct);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPatch("users/{userId}/credentials")]
    [EndpointSummary("Changes user credentials.")]
    [EndpointName("ChangeUserCredentials")]
    public async Task<IActionResult> ChangeCredentials(
        string userId,
        [FromBody] ChangeCredentialsRequest request,
        CancellationToken ct)
    {
        var command = new ChangeUserCredentialsCommand(
            userId,
            request.OldPassword,
            request.NewPassword,
            request.NewUsername);

        var result = await sender.Send(command, ct);
        return result.Match(_ => NoContent(), Problem);
    }

    [HttpGet("users/{userId}")]
    [EndpointSummary("Retrieves a user by ID.")]
    [EndpointName("GetUserById")]
    public async Task<IActionResult> GetUserById(
        string userId,
        CancellationToken ct)
    {
        var query = new GetUserByIdQuery(userId);
        var result = await sender.Send(query, ct);

        return result.Match(Ok, Problem);
    }

    [HttpGet("users")]
    [EndpointSummary("Retrieves all users.")]
    [EndpointName("GetUsers")]
    public async Task<IActionResult> GetUsers(CancellationToken ct)
    {
        var query = new GetUsersQuery();
        var result = await sender.Send(query, ct);

        return result.Match(Ok, Problem);
    }

    [HttpPost("users/{userId}/roles/{roleId}")]
    [EndpointSummary("Assigns a role to a user.")]
    [EndpointName("AssignRoleToUser")]
    public async Task<IActionResult> AssignRole(
        string userId,
        string roleId,
        CancellationToken ct)
    {
        var command = new AssignRoleToUserCommand(userId, roleId);
        var result = await sender.Send(command, ct);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpDelete("users/{userId}/roles/{roleId}")]
    [EndpointSummary("Removes a role from a user.")]
    [EndpointName("RemoveRoleFromUser")]
    public async Task<IActionResult> RemoveRole(
        string userId,
        string roleId,
        CancellationToken ct)
    {
        var command = new RemoveRoleFromUserCommand(userId, roleId);
        var result = await sender.Send(command, ct);

        return result.Match(_ => NoContent(), Problem);
    }

    // =========================
    // Roles
    // =========================

    [HttpPost("roles")]
    [EndpointSummary("Creates a role.")]
    [EndpointName("CreateRole")]
    public async Task<IActionResult> CreateRole(
        [FromBody] CreateRoleRequest request,
        CancellationToken ct)
    {
        var command = new CreateRoleCommand(request.Name);
        var result = await sender.Send(command, ct);

        return result.Match(Ok, Problem);
    }

    [HttpDelete("roles/{roleId}")]
    [EndpointSummary("Deletes a role.")]
    [EndpointName("DeleteRole")]
    public async Task<IActionResult> DeleteRole(
        string roleId,
        CancellationToken ct)
    {
        var command = new DeleteRoleCommand(roleId);
        var result = await sender.Send(command, ct);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPost("roles/{roleId}/permissions")]
    [EndpointSummary("Assigns permissions to a role.")]
    [EndpointName("AssignPermissionsToRole")]
    public async Task<IActionResult> AssignPermissionsToRole(
        string roleId,
        [FromBody] PermissionIdsRequest request,
        CancellationToken ct)
    {
        var command = new AssignPermissionsToRoleCommand(roleId, request.PermissionIds);
        var result = await sender.Send(command, ct);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpDelete("roles/{roleId}/permissions")]
    [EndpointSummary("Removes permissions from a role.")]
    [EndpointName("RemovePermissionsFromRole")]
    public async Task<IActionResult> RemovePermissionsFromRole(
        string roleId,
        [FromBody] PermissionIdsRequest request,
        CancellationToken ct)
    {
        var command = new RemovePermissionsFromRoleCommand(roleId, request.PermissionIds);
        var result = await sender.Send(command, ct);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpGet("roles")]
    [EndpointSummary("Retrieves all roles.")]
    [EndpointName("GetRoles")]
    public async Task<IActionResult> GetRoles(CancellationToken ct)
    {
        var query = new GetRolesQuery();
        var result = await sender.Send(query, ct);

        return result.Match(Ok, Problem);
    }

    // =========================
    // User Permission Overrides
    // =========================

    [HttpPost("users/{userId}/permissions/grant")]
    [EndpointSummary("Grants permissions to a user.")]
    [EndpointName("GrantPermissionsToUser")]
    public async Task<IActionResult> GrantPermissions(
        string userId,
        [FromBody] PermissionIdsRequest request,
        CancellationToken ct)
    {
        var command = new GrantPermissionsToUserCommand(userId, request.PermissionIds);
        var result = await sender.Send(command, ct);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPost("users/{userId}/permissions/deny")]
    [EndpointSummary("Denies permissions to a user.")]
    [EndpointName("DenyPermissionsToUser")]
    public async Task<IActionResult> DenyPermissions(
        string userId,
        [FromBody] PermissionIdsRequest request,
        CancellationToken ct)
    {
        var command = new DenyPermissionsToUserCommand(userId, request.PermissionIds);
        var result = await sender.Send(command, ct);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpDelete("users/{userId}/permissions")]
    [EndpointSummary("Removes permission overrides from a user.")]
    [EndpointName("RemoveUserPermissionOverrides")]
    public async Task<IActionResult> RemovePermissionOverrides(
        string userId,
        [FromBody] PermissionIdsRequest request,
        CancellationToken ct)
    {
        var command = new RemoveUserPermissionOverridesCommand(userId, request.PermissionIds);
        var result = await sender.Send(command, ct);

        return result.Match(_ => NoContent(), Problem);
    }

    // =========================
    // Permissions Queries
    // =========================

    [HttpGet("users/{userId}/permissions")]
    [EndpointSummary("Retrieves effective permissions for a user.")]
    [EndpointName("GetEffectiveUserPermissions")]
    public async Task<IActionResult> GetEffectivePermissions(
        string userId,
        CancellationToken ct)
    {
        var query = new GetEffectiveUserPermissionsQuery(userId);
        var result = await sender.Send(query, ct);

        return result.Match(Ok, Problem);
    }

    [HttpGet("permissions")]
    [EndpointSummary("Retrieves all permissions.")]
    [EndpointName("GetAllPermissions")]
    public async Task<IActionResult> GetAllPermissions(CancellationToken ct)
    {
        var query = new GetAllPermissionsQuery();
        var result = await sender.Send(query, ct);

        return result.Match(Ok, Problem);
    }
}