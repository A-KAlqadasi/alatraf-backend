
using System.Security.Claims;

using AlatrafClinic.Api.Requests.Common;
using AlatrafClinic.Api.Requests.Identity;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Identity;
using AlatrafClinic.Application.Features.Identity.Commands.AddPermissionsToRole;
using AlatrafClinic.Application.Features.Identity.Commands.AddPermissionsToUser;
using AlatrafClinic.Application.Features.Identity.Commands.ChangeUserNameAndPassword;
using AlatrafClinic.Application.Features.Identity.Commands.CreateUser;
using AlatrafClinic.Application.Features.Identity.Commands.RemovePermissionsFromRole;
using AlatrafClinic.Application.Features.Identity.Commands.RemovePermissionsFromUser;
using AlatrafClinic.Application.Features.Identity.Commands.UpdateUser;
using AlatrafClinic.Application.Features.Identity.Dtos;
using AlatrafClinic.Application.Features.Identity.Queries.GenerateTokens;
using AlatrafClinic.Application.Features.Identity.Queries.GetUser;
using AlatrafClinic.Application.Features.Identity.Queries.GetUsers;
using AlatrafClinic.Application.Features.Identity.Queries.RefreshTokens;
using AlatrafClinic.Domain.Common.Results;

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
    [ProducesResponseType(typeof(AppUserDto), StatusCodes.Status200OK)]
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

    [HttpGet("users")]
    [ProducesResponseType(typeof(PaginatedList<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves a paginated list of users.")]
    [EndpointDescription(
        "Supports filtering by search term, username, full name, and active status. " +
        "Results are paginated and sortable."
    )]
    [EndpointName("GetUsers")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> GetUsers(
        [FromQuery] UsersFilterRequest filter,
        [FromQuery] PageRequest pageRequest,
        CancellationToken ct = default)
    {
        var query = new GetUsersQuery(
            pageRequest.Page,
            pageRequest.PageSize,
            filter.SearchTerm,
            filter.UserName,
            filter.FullName,
            filter.IsActive,
            filter.SortColumn,
            filter.SortDirection
        );

        var result = await sender.Send(query, ct);

        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpPost("users")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Creates a new user.")]
    [EndpointDescription("Creates a new user with person information, credentials, and optional roles/permissions, returning the created user details.")]
    [EndpointName("CreateUser")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request, CancellationToken ct = default)
    {
        var result = await sender.Send(new CreateUserCommand(
            request.FullName,
            request.Birthdate,
            request.Phone,
            request.NationalNo,
            request.Address,
            request.Gender,
            request.UserName,
            request.Password,
            request.Permissions,
            request.Roles
        ), ct);

        return result.Match(
            response =>  Ok(response),
            Problem
        );
    }

    [HttpPut("users")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Updates an existing user.")]
    [EndpointDescription("Updates the user personal information and activation status using the unique user identifier.")]
    [EndpointName("UpdateUser")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request, CancellationToken ct = default)
    {
        var result = await sender.Send(new UpdateUserCommand(
            request.UserId,
            request.Fullname,
            request.Birthdate,
            request.Phone,
            request.NationalNo,
            request.Address,
            request.Gender,
            request.IsActive
        ), ct);

        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

    [HttpPatch("users/credentials")]
    [ProducesResponseType(typeof(Success), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Changes a user's username and password.")]
    [EndpointDescription("Updates the username and password for a user, validating the current password before applying changes.")]
    [EndpointName("ChangeUserNameAndPassword")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> ChangeUserNameAndPassword(
        [FromBody] ChangeUserNameAndPasswordRequest request,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new ChangeUserNameAndPasswordCommand(
            request.UserId,
            request.Username,
            request.CurrentPassword,
            request.NewPassword
        ), ct);

        return result.Match(
            _ => Ok(new Success()),
            Problem
        );
    }

    [HttpPut("users/permissions")]
    [ProducesResponseType(typeof(Success), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Adds permissions to a user.")]
    [EndpointDescription("Adds one or more permissions to a user. Missing permissions may be created depending on business rules.")]
    [EndpointName("AddPermissionsToUser")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> AddPermissionsToUser(
        [FromBody] AddPermissionsToUserRequest request,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new AddPermissionsToUserCommand(
            request.UserId,
            request.PermissionNames
        ), ct);

        return result.Match(
            _ => Ok(new Success()),
            Problem
        );
    }

    [HttpDelete("users/permissions")]
    [ProducesResponseType(typeof(Success), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Removes permissions from a user.")]
    [EndpointDescription("Removes one or more permissions from a user. The operation is idempotent.")]
    [EndpointName("RemovePermissionsFromUser")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> RemovePermissionsFromUser(
        [FromBody] RemovePermissionsFromUserRequest request,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new RemovePermissionsFromUserCommand(
            request.UserId,
            request.PermissionNames
        ), ct);

        return result.Match(
            _ => Ok(new Success()),
            Problem
        );
    }

    [HttpPut("roles/permissions")]
    [ProducesResponseType(typeof(Success), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Adds permissions to a role.")]
    [EndpointDescription("Adds one or more permissions to a role. Missing permissions may be created depending on business rules.")]
    [EndpointName("AddPermissionsToRole")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> AddPermissionsToRole(
        [FromBody] AddPermissionsToRoleRequest request,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new AddPermissionsToRoleCommand(
            request.RoleName,
            request.PermissionNames
        ), ct);

        return result.Match(
            _ => Ok(new Success()),
            Problem
        );
    }

    [HttpDelete("roles/permissions")]
    [ProducesResponseType(typeof(Success), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Removes permissions from a role.")]
    [EndpointDescription("Removes one or more permissions from a role. The operation is idempotent.")]
    [EndpointName("RemovePermissionsFromRole")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> RemovePermissionsFromRole(
        [FromBody] RemovePermissionsFromRoleRequest request,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new RemovePermissionsFromRoleCommand(
            request.RoleName,
            request.PermissionNames
        ), ct);

        return result.Match(
            _ => Ok(new Success()),
            Problem
        );
    }

}