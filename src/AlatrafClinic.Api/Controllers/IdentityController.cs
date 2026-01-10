
using System.Security.Claims;

using AlatrafClinic.Api.Requests.Identity;
using AlatrafClinic.Application.Features.Identity;
using AlatrafClinic.Application.Features.Identity.Commands.ActivatePermissionsInRole;
using AlatrafClinic.Application.Features.Identity.Commands.ActivateUser;
using AlatrafClinic.Application.Features.Identity.Commands.AssignRolesToUser;
using AlatrafClinic.Application.Features.Identity.Commands.ChangeUserCredentials;
using AlatrafClinic.Application.Features.Identity.Commands.CreateUser;
using AlatrafClinic.Application.Features.Identity.Commands.DeActivatePermissionsInRole;
using AlatrafClinic.Application.Features.Identity.Commands.DenyPermissionsToUser;
using AlatrafClinic.Application.Features.Identity.Commands.GrantPermissionsToUser;
using AlatrafClinic.Application.Features.Identity.Commands.RemoveRolesFromUser;
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
    public async Task<IActionResult> GenerateToken([FromBody] LoginRequest request, CancellationToken ct)
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
    [ProducesResponseType(typeof(UserCreatedDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Creates a new user.")]
    [EndpointName("CreateUser")]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserRequest request,
        CancellationToken ct)
    {
        var command = new CreateUserCommand(
            request.Fullname,
            request.Birthdate,
            request.Phone,
            request.NationalNo,
            request.Address,
            request.Gender,
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

    [HttpPatch("users/{userId}/change-credentials")]
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
    public async Task<IActionResult> GetUsers([FromQuery] GetUserFilterRequest filter, CancellationToken ct)
    {
        var query = new GetUsersQuery(filter.searchBy, filter.IsActive);
        var result = await sender.Send(query, ct);

        return result.Match(Ok, Problem);
    }

    [HttpPost("users/{userId}/roles")]
    [EndpointSummary("Assigns a role to a user.")]
    [EndpointName("AssignRoleToUser")]
    public async Task<IActionResult> AssignRole(
        string userId,
        AssignRolesRequest request,
        CancellationToken ct)
    {
        var command = new AssignRolesToUserCommand(userId, request.RoleIds);
        var result = await sender.Send(command, ct);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpDelete("users/{userId}/roles")]
    [EndpointSummary("Removes a role from a user.")]
    [EndpointName("RemoveRoleFromUser")]
    public async Task<IActionResult> RemoveRole(
        string userId,
        RemoveRolesRequest request,
        CancellationToken ct)
    {
        var command = new RemoveRolesFromUserCommand(userId, request.RoleIds);
        var result = await sender.Send(command, ct);

        return result.Match(_ => NoContent(), Problem);
    }

    // =========================
    // Roles
    // =========================

    [HttpPatch("roles/{roleId}/permissions/activate")]
    [EndpointSummary("Activate permissions in a role.")]
    [EndpointName("ActivatePermissionsInRole")]
    public async Task<IActionResult> ActivatePermissionsInRole(
        string roleId,
        [FromBody] PermissionIdsRequest request,
        CancellationToken ct)
    {
        var command = new ActivatePermissionsInRoleCommand(roleId, request.PermissionIds);
        var result = await sender.Send(command, ct);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPatch("roles/{roleId}/permissions/deactivate")]
    [EndpointSummary("Deactivate permissions in a role.")]
    [EndpointName("DeactivatePermissionsInRole")]
    public async Task<IActionResult> DeactivatePermissionsInRole(
        string roleId,
        [FromBody] PermissionIdsRequest request,
        CancellationToken ct)
    {
        var command = new DeActivatePermissionsInRoleCommand(roleId, request.PermissionIds);
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
    public async Task<IActionResult> GetAllPermissions([FromQuery] string? search, CancellationToken ct)
    {
        var query = new GetAllPermissionsQuery(search);
        var result = await sender.Send(query, ct);

        return result.Match(Ok, Problem);
    }
}