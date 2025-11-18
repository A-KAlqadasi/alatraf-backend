using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Accounts;

public sealed class Account : AuditableEntity<int>
{
    public string AccountName { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    private Account() { }

    private Account(string name)
    {
        AccountName = name.Trim();
    }

    public static Result<Account> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return AccountErrors.NameRequired;

        if (name.Length > 100)
            return AccountErrors.NameTooLong;

        return new Account(name);
    }

    public Result<Updated> Update(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return AccountErrors.NameRequired;

        if (name.Length > 100)
            return AccountErrors.NameTooLong;

        AccountName = name.Trim();
        return Result.Updated;
    }
}
