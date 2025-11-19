using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Accounts;

public static class AccountErrors
{
    public static readonly Error NameRequired =
        Error.Validation("Account.NameRequired", "Account type name is required.");

    public static readonly Error NameTooLong =
        Error.Validation("Account.NameTooLong", "Account type name must not exceed 100 characters.");
}
