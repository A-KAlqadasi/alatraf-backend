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
}