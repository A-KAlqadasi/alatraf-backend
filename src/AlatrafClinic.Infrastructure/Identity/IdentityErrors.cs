using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Infrastructure.Identity;

public static class MyIdentityErrors
{
    public static readonly Error FailToCreateUser = Error.Failure(description:"User createion is failed");

    public static readonly Error UserNotFound = Error.NotFound(description: "User not found");

    public static readonly Error FailToResetPassword = Error.Failure(description: "Failed to reset password");
    public static readonly Error RoleNotFound = Error.NotFound(description: "Role not found");
    public static readonly Error FailToCreateRole = Error.Failure(description: "Role creation is failed");
    public static readonly Error RoleAssignedToUsers = Error.Conflict(description: "Role has assigned users");
    public static readonly Error FaliedToAssignRoleToUser = Error.Failure(description: "Failed to assign role to user");
    public static readonly Error RoleNameIsRequired = Error.Validation(code: "RoleNameIsRequired", description: "Role name is required");
    public static readonly Error InvalidCredentials = Error.Unauthorized(description: "Invalid username/password provided");
    public static readonly Error FailToChangePassword = Error.Failure(description: "Failed to change password");
    public static readonly Error FailToChangeUsername = Error.Failure(description: "Failed to change username");
    public static readonly Error UsernameAlreadyTaken = Error.Conflict(description: "The username is already taken.");
    public static readonly Error FaliedToAssignRoleToUserPermissions = Error.Failure(description: "Failed to assign permissions to role");
    public static readonly Error FaliedToRemoveRoleFromUser = Error.Failure(description: "Failed to remove role from user");
    public static readonly Error PermissionsNotInRole = Error.NotFound(description: "Some permissions are not assigned to the role");
}