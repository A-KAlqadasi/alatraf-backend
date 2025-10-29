

using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.AccountTypes;

public static class AccountTypeErrors
{
    public static readonly Error NameRequired =
        Error.Validation("AccountType.NameRequired", "Account type name is required.");

    public static readonly Error NameTooLong =
        Error.Validation("AccountType.NameTooLong", "Account type name must not exceed 100 characters.");
}
