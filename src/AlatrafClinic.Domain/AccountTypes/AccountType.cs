

using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.AccountTypes;

    public sealed class AccountType : AuditableEntity<int>
{
    public string AccountName { get; private set; } = string.Empty;

    private AccountType() { }

    private AccountType(string name)
    {
        AccountName = name.Trim();
    }

    public static Result<AccountType> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return AccountTypeErrors.NameRequired;

        if (name.Length > 100)
            return AccountTypeErrors.NameTooLong;

        return new AccountType(name);
    }

    public Result<Updated> Update(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return AccountTypeErrors.NameRequired;

        if (name.Length > 100)
            return AccountTypeErrors.NameTooLong;

        AccountName = name.Trim();
        return Result.Updated;
    }
}
